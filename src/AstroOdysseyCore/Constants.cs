namespace AstroOdysseyCore
{
    public static class Constants
    {
        public const string DatabaseName = "AstroOdyssey";

        public const string Action_Authenticate = "/api/Command/Authenticate";
        public const string Action_SignUp = "/api/Command/SignUp";

        public const string Action_GetGameProfile = "/api/Query/GetGameProfile";

        public static string GetActionName(string action)
        {
            if (action.Contains("/api/Command/", StringComparison.InvariantCulture))
                action = action.Replace("/api/Command/", "");

            if (action.Contains("/api/Query/", StringComparison.InvariantCulture))
                action = action.Replace("/api/Query/", "");

            return action;
        }
    }
}
