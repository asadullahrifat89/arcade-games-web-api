using AstroOdysseyCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using System.Security.Claims;

namespace AstroOdysseyWeb
{
    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
        {
            #region Commands

            app.MapPost(
                pattern: Constants.Action_Authenticate, 
                handler: [AllowAnonymous] async (
                    AuthenticateCommand command,
                    IMediator mediator) =>
            {
                return await mediator.Send(command);

            }).WithName(Constants.GetActionName(Constants.Action_Authenticate));


            app.MapPost(
                pattern: Constants.Action_SignUp, 
                handler: [AllowAnonymous] async (
                    SignupCommand command,
                    IMediator mediator) =>
            {
                return await mediator.Send(command);

            }).WithName(Constants.GetActionName(Constants.Action_SignUp)).RequireAuthorization();

            app.MapPost(
                pattern: Constants.Action_SubmitGameScore, 
                handler: async (
                    SubmitGameScoreCommand command,
                    IMediator mediator) =>
            {
                return await mediator.Send(command);

            }).WithName(Constants.GetActionName(Constants.Action_SubmitGameScore)).RequireAuthorization();

            app.MapPost(
                pattern: Constants.Action_GenerateSession,
                handler: async (
                    GenerateSessionCommand command,
                    IMediator mediator) =>
            {
                return await mediator.Send(command);

            }).WithName(Constants.GetActionName(Constants.Action_GenerateSession)).RequireAuthorization();

            app.MapPost(
                pattern: Constants.Action_ValidateSession, 
                handler: [AllowAnonymous] async (
                    ValidateSessionCommand command,
                    IMediator mediator) =>
            {
                return await mediator.Send(command);

            }).WithName(Constants.GetActionName(Constants.Action_ValidateSession)).RequireAuthorization();

            #endregion

            #region Queries

            app.MapGet(
                pattern: Constants.Action_Ping, 
                handler: [AllowAnonymous] () =>
                {
                    return Results.Ok("I am alive");

                }).WithName(Constants.GetActionName(Constants.Action_Ping));

            app.MapGet(
                pattern: Constants.Action_GetGameProfile, 
                handler: async (
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

            app.MapGet(
                pattern: Constants.Action_GetGameProfiles,
                handler: async (
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

            app.MapGet(
                pattern: Constants.Action_GetGameScores,
                handler: async (
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

            app.MapGet(
                pattern: Constants.Action_GetUser, 
                handler: async (
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor) =>
            {
                return await mediator.Send(new GetUserQuery()
                {
                    UserId = GetUserIdFromHttpContext(httpContextAccessor)
                });

            }).WithName(Constants.GetActionName(Constants.Action_GetUser)).RequireAuthorization(); 

            #endregion

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
