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

        #endregion

        #region Methods

        public async Task<QueryRecordsResponse<GameScore>> GetGameScores(GetGameScoresQuery query)
        {
            var filter = Builders<GameScore>.Filter.Eq(x => x.GameId, query.GameId);

            var scoreDay = query.ScoreDay.ToUniversalTime().Date;
            filter &= Builders<GameScore>.Filter.Eq(x => x.ScoreDay, scoreDay);

            var count = await _mongoDBService.CountDocuments(filter);

            var results = await _mongoDBService.GetDocuments(
                filter: filter,
                skip: query.PageIndex * query.PageSize,
                limit: query.PageSize,
                sortOrder: SortOrder.Descending,
                sortFieldName: nameof(GameScore.Score));

            return new QueryRecordsResponse<GameScore>().BuildSuccessResponse(
                count: results is not null ? count : 0,
                records: results is not null ? results.ToArray() : Array.Empty<GameScore>());
        }

        public async Task<ServiceResponse> SubmitGameScore(SubmitGameScoreCommand command)
        {
            var currentScore = GameScore.Initialize(command);

            // get personal best score before applying current game score
            GameScore? personalBestScore = await GetPersonalBestScore(command);

            // if current game score is greater than personal best score then update it
            double bestScore = personalBestScore is null
                ? currentScore.Score
                : currentScore.Score > personalBestScore.Score ? currentScore.Score : personalBestScore.Score;

            await _gameProfileRepository.UpdateGameProfile(
                score: currentScore.Score,
                bestScore: bestScore,
                userId: currentScore.User.UserId,
                gameId: currentScore.GameId);

            // check if a score exists for the current day
            GameScore? dailyScore = await GetScoreOfTheDay(command);

            // if no score for the day exists then insert new game score for the day
            if (dailyScore is null)
            {
                await _mongoDBService.InsertDocument(currentScore);
            }
            else // if current score beats existing daily score then update
            {
                if (currentScore.Score > dailyScore.Score)
                {
                    var update = Builders<GameScore>.Update
                        .Set(x => x.Score, currentScore.Score)
                        .Set(x => x.ModifiedOn, DateTime.UtcNow);

                    await _mongoDBService.UpdateById(update: update, id: dailyScore.Id);
                }
            }

            return Response.Build().BuildSuccessResponse(currentScore);
        }

        private async Task<GameScore> GetPersonalBestScore(SubmitGameScoreCommand command)
        {
            var filter = Builders<GameScore>.Filter.And(
                Builders<GameScore>.Filter.Eq(x => x.GameId, command.GameId),
                Builders<GameScore>.Filter.Eq(x => x.User.UserId, command.User.UserId));

            var personalBestScore = await _mongoDBService.FindOne(
                filter: filter,
                sortOrder: SortOrder.Descending,
                sortFieldName: nameof(GameScore.Score));

            return personalBestScore;
        }

        private async Task<GameScore> GetScoreOfTheDay(SubmitGameScoreCommand command)
        {
            var today = DateTime.UtcNow.Date;

            var filter = Builders<GameScore>.Filter.And(
                Builders<GameScore>.Filter.Eq(x => x.GameId, command.GameId),
                Builders<GameScore>.Filter.Eq(x => x.User.UserId, command.User.UserId),
                Builders<GameScore>.Filter.Eq(x => x.ScoreDay, today));

            var dailyScore = await _mongoDBService.FindOne(filter: filter);

            return dailyScore;
        }

        #endregion
    }
}
