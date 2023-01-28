using AdventGamesCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AdventGamesWeb
{
    public static class EndpointExtensions
    {
        #region Methods

        #region Public

        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
        {
            #region Commands

            Authenticate(app);

            SignUp(app);

            SubmitGameScore(app);

            GenerateSession(app);

            ValidateToken(app);

            #endregion

            #region Queries

            Ping(app);

            GetGameProfiles(app);

            GetGameProfile(app);

            GetGameScoresOfTheDay(app);

            GetGameHighScores(app);

            GetGameWinners(app);

            GetGamePrizes(app);

            GetGamePrizeOfTheDay(app);

            GetUserProfile(app);

            GetUserProfiles(app);

            CheckIdentityAvailability(app);

            GetSeason(app);

            GetCompany(app);

            GetGameSchedule(app);

            #endregion            

            return app;
        }

        #endregion

        #region Private

        #region Commands

        private static void Authenticate(IEndpointRouteBuilder app)
        {
            app.MapPost(
                pattern: Constants.Action_Authenticate,
                handler: [AllowAnonymous] async (
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor) =>
                {
                    var httpContext = httpContextAccessor.HttpContext;

                    if (httpContext is null)
                        return new ServiceResponse().BuildBadRequestResponse("Bad request");

                    return await mediator.Send(new AuthenticateCommand()
                    {
                        UserName = httpContext.Request.Form["UserName"].ToString(),
                        Password = httpContext.Request.Form["Password"].ToString(),
                        CompanyId = httpContext.Request.Form["CompanyId"].ToString(),
                    });

                }).WithName(Constants.GetActionName(Constants.Action_Authenticate));
        }

        private static void SignUp(IEndpointRouteBuilder app)
        {
            app.MapPost(
                pattern: Constants.Action_SignUp,
                handler: [AllowAnonymous] async (
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor) =>
                {
                    var httpContext = httpContextAccessor.HttpContext;

                    if (httpContext is null)
                        return new ServiceResponse().BuildBadRequestResponse("Bad request");

                    return await mediator.Send(new SignupCommand()
                    {
                        Email = httpContext.Request.Form["Email"].ToString(),
                        FullName = httpContext.Request.Form["FullName"].ToString(),
                        UserName = httpContext.Request.Form["UserName"].ToString(),
                        Password = httpContext.Request.Form["Password"].ToString(),
                        City = httpContext.Request.Form["City"].ToString(),
                        GameId = httpContext.Request.Form["GameId"].ToString(),
                        CompanyId = httpContext.Request.Form["CompanyId"].ToString(),
                        MetaData = new Dictionary<string, string>() { { "SubscribedNewsletters", httpContext.Request.Form["SubscribedNewsletters"].ToString() } }
                    });

                }).WithName(Constants.GetActionName(Constants.Action_SignUp));
        }

        private static void SubmitGameScore(IEndpointRouteBuilder app)
        {
            app.MapPost(
                pattern: Constants.Action_SubmitGameScore,
                handler: async (
                    SubmitGameScoreCommandDto command,
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor) =>
                {
                    var (UserId, CompanyId) = GetUserContextFromHttpContext(httpContextAccessor);

                    return await mediator.Send(new SubmitGameScoreCommand()
                    {
                        CompanyId = CompanyId,
                        GameId = command.GameId,
                        Score = command.Score,
                        User = command.User,
                        SessionId = command.SessionId,
                    });

                }).WithName(Constants.GetActionName(Constants.Action_SubmitGameScore));
        }

        private static void GenerateSession(IEndpointRouteBuilder app)
        {
            app.MapPost(
                pattern: Constants.Action_GenerateSession,
                handler: async (
                    GenerateSessionCommandDto command,
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor) =>
                {
                    var (UserId, CompanyId) = GetUserContextFromHttpContext(httpContextAccessor);

                    return await mediator.Send(new GenerateSessionCommand()
                    {
                        GameId = command.GameId,
                        UserId = command.UserId,
                        CompanyId = CompanyId,
                    });

                }).WithName(Constants.GetActionName(Constants.Action_GenerateSession));
        }

        private static void ValidateToken(IEndpointRouteBuilder app)
        {
            app.MapPost(
                pattern: Constants.Action_ValidateToken,
                handler: [AllowAnonymous] async (
                    ValidateTokenCommand command,
                    IMediator mediator) =>
                {
                    return await mediator.Send(command);

                }).WithName(Constants.GetActionName(Constants.Action_ValidateToken));
        }

        #endregion

        #region Queries

        private static void Ping(IEndpointRouteBuilder app)
        {
            app.MapGet(
                pattern: Constants.Action_Ping,
                handler: [AllowAnonymous] () =>
                {
                    var environemntVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                    return Results.Ok($"{environemntVariable}: I am alive!");

                }).WithName(Constants.GetActionName(Constants.Action_Ping));
        }

        private static void CheckIdentityAvailability(IEndpointRouteBuilder app)
        {
            app.MapPost(
                pattern: Constants.Action_CheckIdentityAvailability,
                handler: [AllowAnonymous] async (
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor) =>
                {
                    var httpContext = httpContextAccessor.HttpContext;

                    if (httpContext is null)
                        return new QueryRecordResponse<bool>().BuildErrorResponse(new ErrorResponse().BuildExternalError("Bad request"));

                    return await mediator.Send(new CheckIdentityAvailabilityQuery()
                    {
                        UserName = httpContext.Request.Form["UserName"].ToString(),
                        Email = httpContext.Request.Form["Email"].ToString(),
                        GameId = httpContext.Request.Form["GameId"].ToString(),
                        CompanyId = httpContext.Request.Form["CompanyId"].ToString(),
                    });

                }).WithName(Constants.GetActionName(Constants.Action_CheckIdentityAvailability));
        }

        private static void GetUserProfile(IEndpointRouteBuilder app)
        {
            app.MapGet(
                pattern: Constants.Action_GetUserProfile,
                handler: async (
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor) =>
                {
                    var (UserId, CompanyId) = GetUserContextFromHttpContext(httpContextAccessor);

                    return await mediator.Send(new GetUserProfileQuery()
                    {
                        UserId = UserId,
                        CompanyId = CompanyId,
                    });

                }).WithName(Constants.GetActionName(Constants.Action_GetUserProfile));
        }

        private static void GetGamePrizeOfTheDay(IEndpointRouteBuilder app)
        {
            app.MapGet(
                pattern: Constants.Action_GetGamePrizeOfTheDay,
                handler: [AllowAnonymous] async (
                    string gameId,
                    //int day,
                    string culture,
                    string companyId,
                    IMediator mediator) =>
                {
                    _ = int.TryParse(DateTime.UtcNow.ToString("dd-MMM-yyyy").Split('-')[0], out int _day); // take the day part

                    return await mediator.Send(new GetGamePrizeOfTheDayQuery()
                    {
                        GameId = gameId,
                        Day = _day,
                        Culture = culture,
                        CompanyId = companyId,
                    });

                }).WithName(Constants.GetActionName(Constants.Action_GetGamePrizeOfTheDay));
        }

        private static void GetGamePrizes(IEndpointRouteBuilder app)
        {
            app.MapGet(
                pattern: Constants.Action_GetGamePrizes,
                handler: async (
                    string gameId,
                    int pageIndex,
                    int pageSize,
                    int? day,
                    string? searchTerm,
                    string? culture,
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor) =>
                {
                    var (UserId, CompanyId) = GetUserContextFromHttpContext(httpContextAccessor);

                    return await mediator.Send(new GetGamePrizesQuery()
                    {
                        GameId = gameId,
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        SearchTerm = searchTerm,
                        Day = day,
                        Culture = culture,
                        CompanyId = CompanyId,
                    });

                }).WithName(Constants.GetActionName(Constants.Action_GetGamePrizes));
        }

        private static void GetGameWinners(IEndpointRouteBuilder app)
        {
            app.MapGet(
               pattern: Constants.Action_GetGameWinners,
               handler: async (
                 string gameId,
                 int limit,
                 HighScoreFilter filter,
                 DateTime? fromDate,
                 DateTime? toDate,
                 IMediator mediator,
                 IHttpContextAccessor httpContextAccessor) =>
               {
                   var (UserId, CompanyId) = GetUserContextFromHttpContext(httpContextAccessor);

                   return await mediator.Send(new GetGameWinnersQuery()
                   {
                       GameId = gameId,
                       FromDate = fromDate,
                       ToDate = toDate,
                       Filter = filter,
                       Limit = limit,
                       CompanyId = CompanyId,
                   });

               }).WithName(Constants.GetActionName(Constants.Action_GetGameWinners));
        }

        private static void GetGameHighScores(IEndpointRouteBuilder app)
        {
            app.MapGet(
                pattern: Constants.Action_GetGameHighScores,
                handler: async (
                    string gameId,
                    int limit,
                    HighScoreFilter filter,
                    DateTime? fromDate,
                    DateTime? toDate,
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor) =>
                {
                    var (UserId, CompanyId) = GetUserContextFromHttpContext(httpContextAccessor);

                    return await mediator.Send(new GetGameHighScoresQuery()
                    {
                        GameId = gameId,
                        FromDate = fromDate,
                        ToDate = toDate,
                        Filter = filter,
                        Limit = limit,
                        CompanyId = CompanyId,
                    });

                }).WithName(Constants.GetActionName(Constants.Action_GetGameHighScores));
        }

        private static void GetGameScoresOfTheDay(IEndpointRouteBuilder app)
        {
            app.MapGet(
                pattern: Constants.Action_GetGameScoresOfTheDay,
                handler: async (
                    int pageIndex,
                    int pageSize,
                    string gameId,
                    //string scoreDay,
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor) =>
                {
                    var _scoreDay = DateTime.UtcNow.ToString("dd-MMM-yyyy");

                    var (UserId, CompanyId) = GetUserContextFromHttpContext(httpContextAccessor);

                    return await mediator.Send(new GetGameScoresQuery()
                    {
                        GameId = gameId,
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        ScoreDay = _scoreDay,
                        CompanyId = CompanyId,
                    });

                }).WithName(Constants.GetActionName(Constants.Action_GetGameScoresOfTheDay));
        }

        private static void GetGameProfiles(IEndpointRouteBuilder app)
        {
            app.MapGet(
                pattern: Constants.Action_GetGameProfiles,
                handler: async (
                    int pageIndex,
                    int pageSize,
                    string gameId,
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor) =>
                {
                    var (UserId, CompanyId) = GetUserContextFromHttpContext(httpContextAccessor);

                    return await mediator.Send(new GetGameProfilesQuery()
                    {
                        GameId = gameId,
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        CompanyId = CompanyId,
                    });

                }).WithName(Constants.GetActionName(Constants.Action_GetGameProfiles));
        }

        private static void GetGameProfile(IEndpointRouteBuilder app)
        {
            app.MapGet(
                pattern: Constants.Action_GetGameProfile,
                handler: async (
                    string gameId,
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor) =>
                {
                    var (UserId, CompanyId) = GetUserContextFromHttpContext(httpContextAccessor);

                    return await mediator.Send(new GetGameProfileQuery()
                    {
                        GameId = gameId,
                        UserId = UserId,
                        CompanyId = CompanyId,
                    });

                }).WithName(Constants.GetActionName(Constants.Action_GetGameProfile));
        }

        private static void GetSeason(IEndpointRouteBuilder app)
        {
            app.MapGet(
                pattern: Constants.Action_GetSeason,
                handler: [AllowAnonymous] async (
                    string companyId,
                    IMediator mediator) =>
                {
                    return await mediator.Send(new GetSeasonQuery()
                    {
                        CompanyId = companyId,
                    });

                }).WithName(Constants.GetActionName(Constants.Action_GetSeason));
        }

        private static void GetCompany(IEndpointRouteBuilder app)
        {
            app.MapGet(
                pattern: Constants.Action_GetCompany,
                handler: [AllowAnonymous] async (
                    string companyId,
                    IMediator mediator) =>
                {
                    return await mediator.Send(new GetCompanyQuery()
                    {
                        CompanyId = companyId,
                    });

                }).WithName(Constants.GetActionName(Constants.Action_GetCompany));
        }

        private static void GetGameSchedule(IEndpointRouteBuilder app)
        {
            app.MapGet(
                pattern: Constants.Action_GetGameSchedule,
                handler: [AllowAnonymous] async (
                    string companyId,
                    string seasonId,
                    IMediator mediator) =>
                {
                    return await mediator.Send(new GetGameScheduleQuery()
                    {
                        CompanyId = companyId,
                        SeasonId = seasonId,
                    });

                }).WithName(Constants.GetActionName(Constants.Action_GetGameSchedule));
        }

        private static void GetUserProfiles(IEndpointRouteBuilder app)
        {
            app.MapGet(
                pattern: Constants.Action_GetUserProfiles,
                handler: async (
                    int pageIndex,
                    int pageSize,
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor) =>
                {
                    var (UserId, CompanyId) = GetUserContextFromHttpContext(httpContextAccessor);

                    return await mediator.Send(new GetUserProfilesQuery()
                    {
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        CompanyId = CompanyId,
                    });

                }).WithName(Constants.GetActionName(Constants.Action_GetUserProfiles));
        }

        #endregion

        #region Misc

        private static (string UserId, string CompanyId) GetUserContextFromHttpContext(IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext?.User.Identity is not ClaimsIdentity identity)
                return (string.Empty, string.Empty);

            IEnumerable<Claim> claims = identity.Claims;

            if (claims is null)
                return (string.Empty, string.Empty);

            var userIdClaim = claims.FirstOrDefault(x => x.Type == "Id");
            var companyIdClaim = claims.FirstOrDefault(x => x.Type == "CompanyId");

            if (userIdClaim is null || companyIdClaim is null)
                return (string.Empty, string.Empty);

            var userId = userIdClaim.Value;
            var comapnyId = companyIdClaim.Value;

            return (userId, comapnyId);
        }

        #endregion

        #endregion

        #endregion
    }
}
