using AstroOdysseyCore.Extensions;
using MongoDB.Driver;

namespace AstroOdysseyCore
{
    public class UserRepository : IUserRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDBService;

        #endregion

        #region Ctor

        public UserRepository(IMongoDbService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        #endregion

        #region Methods

        public async Task<bool> BeAnExistingUserEmail(string userEmail)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Email, userEmail);
            return await _mongoDBService.Exists(filter);
        }

        public async Task<bool> BeAnExistingUserName(string userName)
        {
            var filter = Builders<User>.Filter.Eq(x => x.UserName, userName);
            return await _mongoDBService.Exists(filter);
        }

        public async Task<bool> BeAnExistingUserNameOrEmail(string userNameOrEmail)
        {
            var filter = Builders<User>.Filter.Or(Builders<User>.Filter.Eq(x => x.Email, userNameOrEmail), Builders<User>.Filter.Eq(x => x.UserName, userNameOrEmail));
            return await _mongoDBService.Exists(filter);
        }

        public async Task<bool> BeValidUser(string userNameOrEmail, string password)
        {
            var encryptedPassword = password.Encrypt();

            var filter = Builders<User>.Filter.And(
                   Builders<User>.Filter.Or(Builders<User>.Filter.Eq(x => x.Email, userNameOrEmail), Builders<User>.Filter.Eq(x => x.UserName, userNameOrEmail)),
                   Builders<User>.Filter.Eq(x => x.Password, encryptedPassword));

            return await _mongoDBService.Exists(filter);
        }

        public async Task<GameProfile> Signup(SignupCommand command)
        {
            var user = User.Initialize(command);
            await _mongoDBService.InsertDocument(user);

            var gameProfile = new GameProfile()
            {
                GameId = command.GameId,
                LastGameScore = 0,
                PersonalBestScore = 0,
                User = new AttachedUser()
                {
                    UserId = user.Id,
                    UserName = command.UserName,
                    UserEmail = command.Email,
                },
            };

            await _mongoDBService.InsertDocument(gameProfile);
            return await _mongoDBService.FindOne<GameProfile>(x => x.Id == gameProfile.Id);
        }

        #endregion
    }
}
