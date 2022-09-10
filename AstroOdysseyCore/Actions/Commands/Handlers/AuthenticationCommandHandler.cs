using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AstroOdysseyCore
{
    public class AuthenticationCommandHandler : IRequestHandler<AuthenticationCommand, ServiceResponse>
    {
        #region Fields

        private readonly ILogger<AuthenticationCommandHandler> _logger;
        private readonly AuthenticationCommandValidator _validator;
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;
        #endregion

        #region Ctor

        public AuthenticationCommandHandler(
            ILogger<AuthenticationCommandHandler> logger,
            AuthenticationCommandValidator validator,
            IUserRepository repository,
            IConfiguration configuration)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
            _configuration = configuration;
        }

        #endregion

        #region Methods

        public async Task<ServiceResponse> Handle(AuthenticationCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(command, cancellationToken);
                validationResult.EnsureValidResult();

                var user = await _repository.GetUser(userNameOrEmail: command.UserName, password: command.Password);

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
                        new Claim(JwtRegisteredClaimNames.Sub, command.UserName),
                        new Claim(JwtRegisteredClaimNames.Email, command.UserName),
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

                var result = new AuthToken() { Token = jwtToken, LifeTime = lifeTime };
                return Response.Build().BuildSuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Response.Build().BuildErrorResponse(ex.Message);
            }
        }

        #endregion
    }
}
