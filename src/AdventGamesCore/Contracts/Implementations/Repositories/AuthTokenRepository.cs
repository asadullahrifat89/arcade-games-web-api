using AdventGamesCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdventGamesCore
{
    public class AuthTokenRepository : IAuthTokenRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDBService;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        #endregion

        #region Ctor

        public AuthTokenRepository(
           IMongoDbService mongoDbService,
           IUserRepository userRepository,
           IConfiguration configuration)
        {
            _mongoDBService = mongoDbService;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        #endregion

        #region Methods

        public async Task<ServiceResponse> Authenticate(AuthenticateCommand command)
        {
            var user = await _userRepository.GetUser(
                userNameOrEmail: command.UserName,
                password: command.Password,
                companyId: command.CompanyId);

            AuthToken result = await GenerateAuthToken(userProfile: UserProfile.Initialize(user));

            return Response.Build().BuildSuccessResponse(result);
        }

        public async Task<bool> BeAnExistingRefreshToken(string refreshToken, string companyId)
        {
            var Id = refreshToken.Decrypt();

            var filter = Builders<RefreshToken>.Filter.And(Builders<RefreshToken>.Filter.Eq(x => x.Id, Id), Builders<RefreshToken>.Filter.Eq(x => x.CompanyId, companyId));

            return await _mongoDBService.Exists(filter);
        }

        public async Task<ServiceResponse> ValidateToken(ValidateTokenCommand command)
        {
            var Id = command.RefreshToken.Decrypt();

            var filter = Builders<RefreshToken>.Filter.And(Builders<RefreshToken>.Filter.Eq(x => x.Id, Id), Builders<RefreshToken>.Filter.Eq(x => x.CompanyId, command.CompanyId));

            var refreshToken = await _mongoDBService.FindOne(filter);

            var user = await _userRepository.GetUser(
                userId: refreshToken.UserId,
                companyId: refreshToken.CompanyId);

            if (user == null)
                return Response.Build().BuildErrorResponse("User not found.");

            AuthToken result = await GenerateAuthToken(userProfile: UserProfile.Initialize(user));

            // delete the sent refresh token
            await _mongoDBService.DeleteDocument(filter);

            return Response.Build().BuildSuccessResponse(result);
        }

        private async Task<AuthToken> GenerateAuthToken(UserProfile userProfile)
        {
            var userId = userProfile.UserId;
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = _configuration["Jwt:Key"];

            var keyBytes = Encoding.ASCII.GetBytes(key);

            var lifeTime = DateTime.UtcNow.AddMinutes(2);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", userId),
                    new Claim("CompanyId", userProfile.CompanyId),
                    new Claim(JwtRegisteredClaimNames.Sub, userProfile.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = lifeTime,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            RefreshToken refreshToken = new()
            {
                CompanyId = userProfile.CompanyId,
                UserId = userProfile.UserId
            };

            // save the refresh token
            await _mongoDBService.InsertDocument(refreshToken);

            var result = new AuthToken()
            {
                AccessToken = jwtToken,
                ExpiresOn = lifeTime,
                RefreshToken = refreshToken.Id.Encrypt(),
            };

            return result;
        }

        #endregion
    }
}
