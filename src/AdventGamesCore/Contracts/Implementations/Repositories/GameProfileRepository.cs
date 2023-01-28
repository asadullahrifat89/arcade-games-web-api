using MongoDB.Driver;

namespace AdventGamesCore
{
    public class GameProfileRepository : IGameProfileRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDBService;

        #endregion

        #region Ctor

        public GameProfileRepository(IMongoDbService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordResponse<GameProfile>> GetGameProfile(GetGameProfileQuery query)
        {
            var filter = Builders<GameProfile>.Filter.And(
                Builders<GameProfile>.Filter.Eq(x => x.GameId, query.GameId),
                Builders<GameProfile>.Filter.Eq(x => x.User.UserId, query.UserId),
                Builders<GameProfile>.Filter.Eq(x => x.CompanyId, query.CompanyId));

            var result = await _mongoDBService.FindOne(filter);

            // check if game profile for any other games exists, if so them create the missing game profile
            if (result is null)
            {
                var exists = Builders<GameProfile>.Filter.And(
                    Builders<GameProfile>.Filter.In(x => x.GameId, Constants.GAME_IDS),
                    Builders<GameProfile>.Filter.Eq(x => x.CompanyId, query.CompanyId),
                    Builders<GameProfile>.Filter.Eq(x => x.User.UserId, query.UserId));

                if (await _mongoDBService.Exists(exists))
                {
                    var user = await _mongoDBService.FindById<User>(query.UserId);

                    if (user is not null && await AddGameProfile(new GameProfile()
                    {
                        GameId = query.GameId,
                        LastGameScore = 0,
                        PersonalBestScore = 0,
                        CompanyId = query.CompanyId,
                        User = new AttachedUser()
                        {
                            UserId = user.Id,
                            UserName = user.UserName
                        }
                    }))
                    {
                        result = await _mongoDBService.FindOne(filter);
                    }
                }
            }

            return result is not null
                ? new QueryRecordResponse<GameProfile>().BuildSuccessResponse(result)
                : new QueryRecordResponse<GameProfile>().BuildErrorResponse(new ErrorResponse().BuildExternalError("Game profile not found."));
        }

        public async Task<QueryRecordsResponse<GameProfile>> GetGameProfiles(GetGameProfilesQuery query)
        {
            var filter = Builders<GameProfile>.Filter.And(
                Builders<GameProfile>.Filter.Eq(x => x.GameId, query.GameId),
                Builders<GameProfile>.Filter.Eq(x => x.CompanyId, query.CompanyId),
                Builders<GameProfile>.Filter.Gt(x => x.PersonalBestScore, 0),
                Builders<GameProfile>.Filter.Lt(x => x.PersonalBestScore, double.MaxValue),
                Builders<GameProfile>.Filter.Lt(x => x.PersonalBestScore, 99999));

            var count = await _mongoDBService.CountDocuments(filter);

            var results = await _mongoDBService.GetDocuments(
              filter: filter,
              skip: query.PageIndex * query.PageSize,
              limit: query.PageSize,
              sortOrder: SortOrder.Descending,
              sortFieldName: nameof(GameProfile.PersonalBestScore));

            return new QueryRecordsResponse<GameProfile>().BuildSuccessResponse(
               count: results is not null ? count : 0,
               records: results is not null ? results.ToArray() : Array.Empty<GameProfile>());
        }

        public async Task<bool> AddGameProfile(GameProfile gameProfile)
        {
            return await _mongoDBService.InsertDocument(gameProfile);
        }

        public async Task<bool> UpdateGameProfile(double score, double bestScore, string userId, string gameId, string companyId)
        {
            var filter = Builders<GameProfile>.Filter.And(
                  Builders<GameProfile>.Filter.Eq(x => x.GameId, gameId),
                  Builders<GameProfile>.Filter.Eq(x => x.User.UserId, userId),
                  Builders<GameProfile>.Filter.Eq(x => x.CompanyId, companyId));

            var updated = await _mongoDBService.UpdateDocument(
                update: Builders<GameProfile>.Update
                .Set(x => x.PersonalBestScore, bestScore)
                .Set(x => x.LastGameScore, score)
                .Set(x => x.ModifiedOn, DateTime.UtcNow),
                filter: filter);

            return updated is not null;
        }

        public async Task BanHackers()
        {
            var filter = Builders<GameProfile>.Filter.Or(
               Builders<GameProfile>.Filter.Lt(x => x.PersonalBestScore, 0),
               Builders<GameProfile>.Filter.Gt(x => x.PersonalBestScore, 99999),
               Builders<GameProfile>.Filter.Eq(x => x.PersonalBestScore, double.MaxValue));

            var count = await _mongoDBService.CountDocuments(filter);

            if (count > 0)
            {
                await _mongoDBService.DeleteDocuments(filter);
            }
        }

        #endregion
    }
}
