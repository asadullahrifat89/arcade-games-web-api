using AdventGamesCore.Extensions;
using MongoDB.Driver;

namespace AdventGamesCore
{
    public class UserRepository : IUserRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDbService;

        #endregion

        #region Ctor

        public UserRepository(IMongoDbService mongoDBService)
        {
            _mongoDbService = mongoDBService;
        }

        #endregion

        #region Methods

        public async Task<bool> BeAnExistingUser(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            return await _mongoDbService.Exists(filter);
        }

        public async Task<bool> BeAnExistingUserEmail(string userEmail, string companyId)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(x => x.Email, userEmail),
                Builders<User>.Filter.Eq(x => x.CompanyId, companyId));

            return await _mongoDbService.Exists(filter);
        }

        public async Task<bool> BeAnExistingUserName(string userName, string companyId)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(x => x.UserName, userName),
                Builders<User>.Filter.Eq(x => x.CompanyId, companyId));

            return await _mongoDbService.Exists(filter);
        }

        public async Task<bool> BeAnExistingUserNameOrEmail(string userNameOrEmail, string companyId = "")
        {
            var filter =
                Builders<User>.Filter.Or(
                    Builders<User>.Filter.Eq(x => x.Email, userNameOrEmail),
                    Builders<User>.Filter.Eq(x => x.UserName, userNameOrEmail));

            if (!companyId.IsNullOrBlank())
                filter &= Builders<User>.Filter.Eq(x => x.CompanyId, companyId);

            return await _mongoDbService.Exists(filter);
        }

        public async Task<bool> BeValidUser(string userNameOrEmail, string password)
        {
            var encryptedPassword = password.Encrypt();

            var filter = Builders<User>.Filter.And(
                   Builders<User>.Filter.Or(Builders<User>.Filter.Eq(x => x.Email, userNameOrEmail), Builders<User>.Filter.Eq(x => x.UserName, userNameOrEmail)),
                   Builders<User>.Filter.Eq(x => x.Password, encryptedPassword));

            return await _mongoDbService.Exists(filter);
        }

        public async Task<User> GetUser(string userNameOrEmail, string password, string companyId)
        {
            var encryptedPassword = password.Encrypt();

            var filter = Builders<User>.Filter.And(
                   Builders<User>.Filter.Or(Builders<User>.Filter.Eq(x => x.Email, userNameOrEmail), Builders<User>.Filter.Eq(x => x.UserName, userNameOrEmail)),
                   Builders<User>.Filter.Eq(x => x.Password, encryptedPassword),
                   Builders<User>.Filter.Eq(x => x.CompanyId, companyId));

            return await _mongoDbService.FindOne(filter);
        }

        public async Task<QueryRecordResponse<UserProfile>> GetUserProfile(GetUserProfileQuery query)
        {
            var user = await _mongoDbService.FindOne<User>(x => x.Id == query.UserId && x.CompanyId == query.CompanyId);

            return user is null
                ? new QueryRecordResponse<UserProfile>().BuildErrorResponse(new ErrorResponse().BuildExternalError("User doesn't exist."))
                : new QueryRecordResponse<UserProfile>().BuildSuccessResponse(UserProfile.Initialize(user));
        }

        public async Task<QueryRecordsResponse<UserProfile>> GetUserProfiles(GetUserProfilesQuery query)
        {
            var filter = Builders<User>.Filter.Eq(x => x.CompanyId, query.CompanyId);

            var count = await _mongoDbService.CountDocuments(filter);

            var results = await _mongoDbService.GetDocuments(
              filter: filter,
              skip: query.PageIndex * query.PageSize,
              limit: query.PageSize,
              sortOrder: SortOrder.Descending,
              sortFieldName: nameof(User.CreatedOn));

            return new QueryRecordsResponse<UserProfile>().BuildSuccessResponse(
               count: results is not null ? count : 0,
               records: results is not null ? results.Select(UserProfile.Initialize).ToArray() : Array.Empty<UserProfile>());
        }

        public async Task<User[]> GetUsers(string[] userIds, string companyId)
        {
            var filter = Builders<User>.Filter.And(Builders<User>.Filter.Eq(x => x.CompanyId, companyId), Builders<User>.Filter.In(x => x.Id, userIds));

            var results = await _mongoDbService.GetDocuments(filter: filter);

            return results is null ? Array.Empty<User>() : results.ToArray();
        }

        public async Task<User> GetUser(string userId, string companyId)
        {
            var filter = Builders<User>.Filter.And(Builders<User>.Filter.Eq(x => x.CompanyId, companyId), Builders<User>.Filter.Eq(x => x.Id, userId));

            return await _mongoDbService.FindOne(filter);
        }

        public async Task<ServiceResponse> Signup(SignupCommand command)
        {
            var user = User.Initialize(command);

            await _mongoDbService.InsertDocument(user);

            // create game profiles based on game ids
            GameProfile[] gameProfiles = Constants.GAME_IDS.Select(gameId => GameProfile.Initialize(
                command: command,
                userId: user.Id,
                gameId: gameId)).ToArray();

            await _mongoDbService.InsertDocuments(gameProfiles);

            return Response.Build().BuildSuccessResponse(gameProfiles.First(x => x.GameId == command.GameId));
        }

        #endregion
    }
}
