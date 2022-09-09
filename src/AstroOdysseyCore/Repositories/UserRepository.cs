using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroOdysseyCore
{
    public class UserRepository : IUserRepository
    {
        public UserRepository()
        {

        }

        public Task<bool> BeAnExistingUser(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BeValidUser(string email, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<GameProfile> Signup(SignupCommand command)
        {
            //TODO: save user in database, create game profile for user
            return new GameProfile()
            {
                GameId = command.GameId,
                LastGameScore = 0,
                PersonalBestScore = 0,
                User = new AttachedUser()
                {
                    UserEmail = command.Email,
                    UserId = Guid.NewGuid().ToString(),
                    UserName = command.UserName,
                },
            };
        }
    }
}
