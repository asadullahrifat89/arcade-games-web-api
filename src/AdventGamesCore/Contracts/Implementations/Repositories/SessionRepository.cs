using AdventGamesCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdventGamesCore
{
    public class SessionRepository : ISessionRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDBService;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        #endregion

        #region Ctor

        public SessionRepository(
            IMongoDbService mongoDBService,
            IUserRepository userRepository,
            IConfiguration configuration)
        {
            _mongoDBService = mongoDBService;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        #endregion

        #region Methods

        public async Task<bool> BeAnExistingSession(string sessionId)
        {
            // decrypt session id and match with database
            var SessionId = sessionId.Decrypt();

            var filter = Builders<Session>.Filter.Eq(x => x.SessionId, SessionId);
            return await _mongoDBService.Exists(filter);
        }


        public async Task<ServiceResponse> Authenticate(AuthenticateCommand command)
        {
            var user = await _userRepository.GetUser(userNameOrEmail: command.UserName, password: command.Password);

            AuthToken result = GenerateAuthToken(user);

            return Response.Build().BuildSuccessResponse(result);
        }

        public async Task<ServiceResponse> GenerateSession(GenerateSessionCommand command)
        {
            // save raw data
            var session = Session.Initialize(command);
            await _mongoDBService.InsertDocument(session);

            // send back encrypted data
            session.SessionId = session.SessionId.Encrypt();
            session.UserId = session.UserId.Encrypt();

            return Response.Build().BuildSuccessResponse(session);
        }

        public async Task<ServiceResponse> ValidateSession(ValidateSessionCommand command)
        {
            // decrypt session id and match with database
            var sessionId = command.SessionId.Decrypt();

            var session = await _mongoDBService.FindOne<Session>(x => x.SessionId == sessionId && x.GameId == command.GameId);

            if (session is null || DateTime.UtcNow > session.ExpiresOn)
                return Response.Build().BuildErrorResponse("Session expired."); // user will be forced to login again

            // get user details
            var response = await _userRepository.GetUser(new GetUserQuery() { UserId = session.UserId });

            if (!response.IsSuccess)
                return Response.Build().BuildErrorResponse(String.Join("\n", response.Errors.Errors)); // user will be forced to login again

            var user = response.Result;

            if (user is null)
                return Response.Build().BuildErrorResponse("User not found.");

            AuthToken result = GenerateAuthToken(user);

            return Response.Build().BuildSuccessResponse(result);
        }

        private AuthToken GenerateAuthToken(User user)
        {
            var id = user.Id;
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var lifeTime = DateTime.UtcNow.AddMinutes(2);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim("Id", id),
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = lifeTime,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            var result = new AuthToken() { AccessToken = jwtToken, ExpiresOn = lifeTime };
            return result;
        }

        #endregion
    }
}
