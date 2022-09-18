using MongoDB.Driver;

namespace AstroOdysseyCore
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
                Builders<GameProfile>.Filter.Eq(x => x.User.UserId, query.UserId));

            var result = await _mongoDBService.FindOne(filter);

            return result is not null
                ? new QueryRecordResponse<GameProfile>().BuildSuccessResponse(result)
                : new QueryRecordResponse<GameProfile>().BuildErrorResponse(new ErrorResponse().BuildExternalError("Game profile not found."));
        }

        public async Task<QueryRecordsResponse<GameProfile>> GetGameProfiles(GetGameProfilesQuery query)
        {
            var filter = Builders<GameProfile>.Filter.Eq(x => x.GameId, query.GameId);

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

        public async Task<bool> UpdateGameProfile(double score, double bestScore, string userId, string gameId)
        {
            var filter = Builders<GameProfile>.Filter.And(
                  Builders<GameProfile>.Filter.Eq(x => x.GameId, gameId),
                  Builders<GameProfile>.Filter.Eq(x => x.User.UserId, userId));

            var updated = await _mongoDBService.UpdateDocument(
                update: Builders<GameProfile>.Update
                .Set(x => x.PersonalBestScore, bestScore)
                .Set(x => x.LastGameScore, score)
                .Set(x => x.ModifiedOn, DateTime.UtcNow),
                filter: filter);

            return updated is not null;
        }

        #endregion
    }
}
