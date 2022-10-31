namespace SpaceShooterCore
{
    public static class Constants
    {
        public const string DatabaseName = "AdventArcadeGames";

        public const string Action_Ping = "/api/Query/Ping";

        public const string Action_Authenticate = "/api/Command/Authenticate";
        public const string Action_SignUp = "/api/Command/SignUp";
        public const string Action_SubmitGameScore = "/api/Command/SubmitGameScore";
        public const string Action_GenerateSession = "/api/Command/GenerateSession";
        public const string Action_ValidateSession = "/api/Command/ValidateSession";

        public const string Action_GetGameProfile = "/api/Query/GetGameProfile";
        public const string Action_GetGameProfiles = "/api/Query/GetGameProfiles";
        public const string Action_GetGameScores = "/api/Query/GetGameScores";
        public const string Action_GetUser = "/api/Query/GetUser";
        public const string Action_CheckIdentityAvailability = "/api/Query/CheckIdentityAvailability";

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
            "sky-racer",
            "hungry-worm",
            "candy-craze"
        };

        public static string[] Client_Origins = new string[]
        {
            "https://asadullahrifat89.github.io"
        };
    }
}
