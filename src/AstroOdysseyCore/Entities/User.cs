using AstroOdysseyCore.Extensions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroOdysseyCore
{
    public class User : EntityBase
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public static User Initialize(SignupCommand command)
        {
            var encryptedPassword = command.Password.Encrypt();

            var user = new User()
            {
                UserName = command.UserName,
                Email = command.Email,
                Password = encryptedPassword,
            };

            return user;
        }
    }
}
