using AdventGamesCore.Extensions;
using MongoDB.Driver;
using System.Collections.Generic;

namespace AdventGamesCore
{
    public class UserRepository : IUserRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDBService;
        private readonly IGameProfileRepository _gameProfileRepository;

        #endregion

        #region Ctor

        public UserRepository(IMongoDbService mongoDBService, IGameProfileRepository gameProfileRepository)
        {
            _mongoDBService = mongoDBService;
            _gameProfileRepository = gameProfileRepository;
        }

        #endregion

        #region Methods

        public async Task<bool> BeAnExistingUser(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            return await _mongoDBService.Exists(filter);
        }

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

        public async Task<User> GetUser(string userNameOrEmail, string password)
        {
            var encryptedPassword = password.Encrypt();

            var filter = Builders<User>.Filter.And(
                   Builders<User>.Filter.Or(Builders<User>.Filter.Eq(x => x.Email, userNameOrEmail), Builders<User>.Filter.Eq(x => x.UserName, userNameOrEmail)),
                   Builders<User>.Filter.Eq(x => x.Password, encryptedPassword));

            return await _mongoDBService.FindOne(filter);
        }

        public async Task<QueryRecordResponse<User>> GetUser(GetUserQuery query)
        {
            var user = await _mongoDBService.FindById<User>(query.UserId);
            return user is null
                ? new QueryRecordResponse<User>().BuildErrorResponse(new ErrorResponse().BuildExternalError("User doesn't exist."))
                : new QueryRecordResponse<User>().BuildSuccessResponse(user);
        }

        public async Task<User[]> GetUsers(string[] userIds)
        {
            var filter = Builders<User>.Filter.In(x => x.Id, userIds);

            var results = await _mongoDBService.GetDocuments(filter: filter);

            return results is null? Array.Empty<User>(): results.ToArray();
        }

        public async Task<ServiceResponse> Signup(SignupCommand command)
        {
            var user = User.Initialize(command);
            await _mongoDBService.InsertDocument(user);

            // create game profiles based on game ids
            GameProfile[] gameProfiles = Constants.GAME_IDS.Select(gameId => GameProfile.Initialize(
                command: command,
                userId: user.Id,
                gameId: gameId)).ToArray();

            foreach (var gameProfile in gameProfiles)
            {
                await _gameProfileRepository.AddGameProfile(gameProfile);
            }

            return Response.Build().BuildSuccessResponse(gameProfiles.First(x => x.GameId == command.GameId));
        }

        #endregion
    }
}
