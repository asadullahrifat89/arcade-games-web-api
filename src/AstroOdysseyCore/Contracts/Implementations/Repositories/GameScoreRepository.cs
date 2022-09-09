using MongoDB.Driver;

namespace AstroOdysseyCore
{
    public class GameScoreRepository : IGameScoreRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDBService;
        private readonly IGameProfileRepository _gameProfileRepository;

        #endregion

        #region Ctor
        public GameScoreRepository(IMongoDbService mongoDBService, IGameProfileRepository gameProfileRepository)
        {
            _mongoDBService = mongoDBService;
            _gameProfileRepository = gameProfileRepository;
        }

        public async Task<ServiceResponse> SubmitGameScore(SubmitGameScoreCommand command)
        {
            // get personal best score first before this game
            var filter = Builders<GameScore>.Filter.And(
                Builders<GameScore>.Filter.Eq(x => x.GameId, command.GameId),
                Builders<GameScore>.Filter.Eq(x => x.User.UserId, command.User.UserId));

            var personalBestScore = await _mongoDBService.FindOne(filter: filter, sortOrder: SortOrder.Descending, sortFieldName: nameof(GameScore.Score));

            var gameScore = GameScore.Initialize(command);            

            // if current game score is greater than personal best score then update it
            var bestScore = gameScore.Score >= personalBestScore.Score ? gameScore.Score : personalBestScore.Score;

            await _mongoDBService.InsertDocument(gameScore);
            await _gameProfileRepository.UpdateGameProfile(
                score: gameScore.Score,
                bestScore: bestScore,
                userId: gameScore.User.UserId,
                gameId: gameScore.GameId);

            return Response.Build().BuildSuccessResponse(gameScore);
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
