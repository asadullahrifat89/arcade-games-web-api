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

        #endregion
    }
}
