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

            return new QueryRecordResponse<GameProfile>().BuildSuccessResponse(result);
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
