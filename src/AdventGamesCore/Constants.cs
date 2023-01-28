namespace AdventGamesCore
{
    public static class Constants
    {
        public const string Action_Ping = "/api/Query/Ping";

        public const string Action_Authenticate = "/api/Command/Authenticate";
        public const string Action_SignUp = "/api/Command/SignUp";
        public const string Action_SubmitGameScore = "/api/Command/SubmitGameScore";
        public const string Action_GenerateSession = "/api/Command/GenerateSession";
        public const string Action_ValidateToken = "/api/Command/ValidateToken";

        public const string Action_GetGameProfile = "/api/Query/GetGameProfile";
        public const string Action_GetGameProfiles = "/api/Query/GetGameProfiles";
        public const string Action_GetGameScoresOfTheDay = "/api/Query/GetGameScoresOfTheDay";
        public const string Action_GetUserProfile = "/api/Query/GetUserProfile";
        public const string Action_GetUserProfiles = "/api/Query/GetUserProfiles";
        public const string Action_CheckIdentityAvailability = "/api/Query/CheckIdentityAvailability";
        public const string Action_GetGameHighScores = "/api/Query/GetGameHighScores";
        public const string Action_GetGamePrizes = "/api/Query/GetGamePrizes";
        public const string Action_GetGamePrizeOfTheDay = "/api/Query/GetGamePrizeOfTheDay";
        public const string Action_GetGameWinners = "/api/Query/GetGameWinners";
        public const string Action_GetSeason = "/api/Query/GetSeason";
        public const string Action_GetCompany = "/api/Query/GetCompany";
        public const string Action_GetGameSchedule = "/api/Query/GetGameSchedule";

        public static string GetActionName(string action)
        {
            if (action.Contains("/api/Command/", StringComparison.InvariantCulture))
                action = action.Replace("/api/Command/", "");

            if (action.Contains("/api/Query/", StringComparison.InvariantCulture))
                action = action.Replace("/api/Query/", "");

            return action;
        }

        public static string[] GAME_IDS = new string[]
        {
            "space-shooter",
            "sky-way",
            "hungry-worm",
            "candy-craze",
            "memory-match",
            "honk-hero"
        };

        public static string[] Client_Origins = new string[]
        {
            "https://*.seliselocal.com",
            "https://*.adventgames.ch",
            "https://asadullahrifat89.github.io"
        };

        public static string[] AllowedSwaggerEnvironments = new[] { "Development", "dev-az" };
    }
}

