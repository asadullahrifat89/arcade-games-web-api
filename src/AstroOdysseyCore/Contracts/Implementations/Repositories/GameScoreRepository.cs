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

        //public async Task<QueryRecordResponse<GameScore>> GetGameScore(GetGameScoreQuery query)
        //{
        //    var filter = Builders<GameScore>.Filter.And(
        //        Builders<GameScore>.Filter.Eq(x => x.GameId, query.GameId),
        //        Builders<GameScore>.Filter.Eq(x => x.User.UserId, query.UserId));

        //    var result = await _mongoDBService.FindOne(filter);

        //    return new QueryRecordResponse<GameScore>().BuildSuccessResponse(result);
        //}

        #endregion
    }
}
