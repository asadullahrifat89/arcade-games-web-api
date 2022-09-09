using AstroOdysseyCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AstroOdysseyWeb
{
    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost(Constants.Action_Authenticate, [AllowAnonymous] async (AuthenticationCommand command, IConfiguration configuration, AuthenticationCommandValidator validator) =>
            {
                var validationResult = await validator.ValidateAsync(command);

                if (!validationResult.IsValid)
                    return Response.Build().WithErrors(validationResult.Errors.Select(x => x.ErrorMessage).ToArray());

                var issuer = configuration["Jwt:Issuer"];
                var audience = configuration["Jwt:Audience"];
                var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);

                var lifeTime = DateTime.UtcNow.AddMinutes(2);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                            new Claim("Id", Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Sub, command.UserName),
                            new Claim(JwtRegisteredClaimNames.Email, command.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti,
                            Guid.NewGuid().ToString())
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
                return Response.Build().WithResult(result);

            }).WithName(Constants.GetActionName(Constants.Action_Authenticate));

            app.MapPost(Constants.Action_SignUp, [AllowAnonymous] async (SignupCommand command, IMediator mediator) =>
            {
                return await mediator.Send(command);

            }).WithName(Constants.GetActionName(Constants.Action_SignUp)).RequireAuthorization();

            app.MapGet(Constants.Action_GetGameProfile, async (string gameId, string userId, IMediator mediator) =>
            {
                return await mediator.Send(new GetGameProfileQuery() { GameId = gameId, UserId = userId });

            }).WithName(Constants.GetActionName(Constants.Action_GetGameProfile)).RequireAuthorization();

            return app;
        }

        internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
        {
            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        }
    }
}
