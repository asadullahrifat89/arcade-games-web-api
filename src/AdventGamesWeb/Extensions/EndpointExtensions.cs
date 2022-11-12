using AdventGamesCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using System.Security.Claims;

namespace AdventGamesWeb
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

            }).WithName(Constants.GetActionName(Constants.Action_SignUp));

            app.MapPost(
                pattern: Constants.Action_SubmitGameScore,
                handler: async (
                    SubmitGameScoreCommand command,
                    IMediator mediator) =>
            {
                return await mediator.Send(command);

            }).WithName(Constants.GetActionName(Constants.Action_SubmitGameScore));

            app.MapPost(
                pattern: Constants.Action_GenerateSession,
                handler: async (
                    GenerateSessionCommand command,
                    IMediator mediator) =>
            {
                return await mediator.Send(command);

            }).WithName(Constants.GetActionName(Constants.Action_GenerateSession));

            app.MapPost(
                pattern: Constants.Action_ValidateSession,
                handler: [AllowAnonymous] async (
                    ValidateSessionCommand command,
                    IMediator mediator) =>
            {
                return await mediator.Send(command);

            }).WithName(Constants.GetActionName(Constants.Action_ValidateSession));

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

            }).WithName(Constants.GetActionName(Constants.Action_GetGameProfile));

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

            }).WithName(Constants.GetActionName(Constants.Action_GetGameProfiles));

            app.MapGet(
                pattern: Constants.Action_GetGameScores,
                handler: async (
                    int pageIndex,
                    int pageSize,
                    string gameId,
                    string scoreDay,
                    IMediator mediator) =>
                {
                    return await mediator.Send(new GetGameScoresQuery()
                    {
                        GameId = gameId,
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        ScoreDay = scoreDay
                    });

                }).WithName(Constants.GetActionName(Constants.Action_GetGameScores));

            app.MapGet(
                pattern: Constants.Action_GetGameHighScores,
                handler: async (
                  string gameId,
                  int limit,
                  HighScoreFilter filter,
                  DateTime? fromDate,
                  DateTime? toDate,
                  IMediator mediator) =>
                {
                    return await mediator.Send(new GetGameHighScoresQuery()
                    {
                        GameId = gameId,
                        FromDate = fromDate,
                        ToDate = toDate,
                        Filter = filter,
                        Limit = limit,
                    });

                }).WithName(Constants.GetActionName(Constants.Action_GetGameHighScores));

            app.MapGet(
               pattern: Constants.Action_GetGameWinners,
               handler: async (
                 string gameId,
                 int limit,
                 HighScoreFilter filter,
                 DateTime? fromDate,
                 DateTime? toDate,
                 IMediator mediator) =>
               {
                   return await mediator.Send(new GetGameWinnersQuery()
                   {
                       GameId = gameId,
                       FromDate = fromDate,
                       ToDate = toDate,
                       Filter = filter,
                       Limit = limit,
                   });

               }).WithName(Constants.GetActionName(Constants.Action_GetGameWinners));

            app.MapGet(
                pattern: Constants.Action_GetGamePrizes,
                handler: async (
                    string gameId,
                    int pageIndex,
                    int pageSize,
                    int? day,
                    string? searchTerm,
                    string? culture,
                    IMediator mediator) =>
            {
                return await mediator.Send(new GetGamePrizesQuery()
                {
                    GameId = gameId,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    Day = day,
                    Culture = culture,
                });

            }).WithName(Constants.GetActionName(Constants.Action_GetGamePrizes));

            app.MapGet(
             pattern: Constants.Action_GetGamePrize,
             handler: [AllowAnonymous] async (
                 string gameId,
                 int day,
                 string culture,
                 IMediator mediator) =>
             {
                 return await mediator.Send(new GetGamePrizeQuery()
                 {
                     GameId = gameId,
                     Day = day,
                     Culture = culture,
                 });

             }).WithName(Constants.GetActionName(Constants.Action_GetGamePrize));

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

            }).WithName(Constants.GetActionName(Constants.Action_GetUser));

            app.MapGet(
              pattern: Constants.Action_CheckIdentityAvailability,
              handler: [AllowAnonymous] async (
                  string userName,
                  string email,
                  string gameId,
                  IMediator mediator) =>
              {
                  return await mediator.Send(new CheckIdentityAvailabilityQuery()
                  {
                      UserName = userName,
                      Email = email,
                      GameId = gameId,
                  });

              }).WithName(Constants.GetActionName(Constants.Action_CheckIdentityAvailability));

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

            if (claims is null)
                return string.Empty;

            var claim = claims.FirstOrDefault(x => x.Type == "Id");

            if (claim is null)
                return string.Empty;

            var userId = claim.Value;
            return userId;
        }
    }
}
