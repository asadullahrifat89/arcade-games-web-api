using MongoDB.Driver;

namespace AstroOdysseyCore
{
    public class GameScoreRepository : IGameScoreRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDBService;

        #endregion

        #region Ctor
        public GameScoreRepository(IMongoDbService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        public async Task<ActionCommandResponse> SubmitGameScore(SubmitGameScoreCommand command)
        {
            var gameScore = GameScore.Initialize(command);

            await _mongoDBService.InsertDocument(gameScore);

            return Response.Build().WithResult(gameScore);
        }

        #endregion

        #region Methods

        public async Task<QueryRecordsResponse<GameScore>> GetGameScores(GetGameScoresQuery query)
        {
            var filter = Builders<GameScore>.Filter.Eq(x => x.GameId, query.GameId);

            if (query.Since is not null)
            {
                filter &= Builders<GameScore>.Filter.Gte(x => x.CreatedOn, query.Since);
            }

            var count = await _mongoDBService.CountDocuments(filter);
            var results = await _mongoDBService.GetDocuments(filter: filter, skip: query.PageIndex * query.PageSize, limit: query.PageSize);

            return new QueryRecordsResponse<GameScore>().BuildSuccessResponse(
                count: results is not null ? count : 0,
                records: results is not null ? results.ToArray() : Array.Empty<GameScore>());
        }

        #endregion
    }
}
