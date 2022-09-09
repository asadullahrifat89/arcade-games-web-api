using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroOdysseyCore
{
    public static class Constants
    {
        public const string DatabaseName = "AstroOdyssey";

        public static string GetActionName(string action)
        {
            if (action.Contains("/api/Command/"))
                action = action.Replace("/api/Command/", "");

            if (action.Contains("/api/Query/"))
                action = action.Replace("/api/Query/", "");

            return action;
        }
    }
}
