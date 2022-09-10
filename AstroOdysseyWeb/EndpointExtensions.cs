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
            app.MapGet(Constants.Action_Ping, [AllowAnonymous] () =>
            {
                return Results.Ok("I am alive");

            }).WithName(Constants.GetActionName(Constants.Action_Ping));

            app.MapPost(Constants.Action_Authenticate, [AllowAnonymous] async (
                AuthenticationCommand command,
                IMediator mediator) =>
            {
                return await mediator.Send(command);

            }).WithName(Constants.GetActionName(Constants.Action_Authenticate));

            app.MapPost(Constants.Action_SignUp, [AllowAnonymous] async (
                SignupCommand command,
                IMediator mediator) =>
            {
                return await mediator.Send(command);

            }).WithName(Constants.GetActionName(Constants.Action_SignUp)).RequireAuthorization();

            app.MapPost(Constants.Action_SubmitGameScore, async (
                SubmitGameScoreCommand command,
                IMediator mediator) =>
            {
                return await mediator.Send(command);

            }).WithName(Constants.GetActionName(Constants.Action_SubmitGameScore)).RequireAuthorization();

            app.MapGet(Constants.Action_GetGameProfile, async (
                string gameId,
                IMediator mediator,
                IHttpContextAccessor httpContextAccessor) =>
            {
                return await mediator.Send(new GetGameProfileQuery()
                {
                    GameId = gameId,
                    UserId = GetUserIdFromHttpContext(httpContextAccessor)
                });

            }).WithName(Constants.GetActionName(Constants.Action_GetGameProfile)).RequireAuthorization();

            app.MapGet(Constants.Action_GetGameProfiles, async (
                int pageIndex,
                int pageSize,
                string gameId,
                IMediator mediator) =>
            {
                return await mediator.Send(new GetGameProfilesQuery()
                {
                    GameId = gameId,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                });

            }).WithName(Constants.GetActionName(Constants.Action_GetGameProfiles)).RequireAuthorization();

            app.MapGet(Constants.Action_GetGameScores, async (
                int pageIndex,
                int pageSize,
                string gameId,
                DateTime? since,
                IMediator mediator) =>
            {
                return await mediator.Send(new GetGameScoresQuery()
                {
                    GameId = gameId,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Since = since
                });

            }).WithName(Constants.GetActionName(Constants.Action_GetGameScores)).RequireAuthorization();

            app.MapGet(Constants.Action_GetUser, async (
                IMediator mediator,
                IHttpContextAccessor httpContextAccessor) =>
            {
                return await mediator.Send(new GetUserQuery()
                {
                    UserId = GetUserIdFromHttpContext(httpContextAccessor)
                });

            }).WithName(Constants.GetActionName(Constants.Action_GetUser)).RequireAuthorization();

            return app;
        }

        private static string GetUserIdFromHttpContext(IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var identity = httpContext?.User.Identity as ClaimsIdentity;

            if (identity is null)
                return string.Empty;

            IEnumerable<Claim> claims = identity.Claims;

            var userId = claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
            return userId;
        }
    }
}
