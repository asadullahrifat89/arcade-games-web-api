namespace AstroOdysseyCore
{
    public static class Constants
    {
        public const string DatabaseName = "ArcadeStation";

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
            "astro-odyssey",
            "sky-way",
            "hungry-worm",
        };
    }
}
