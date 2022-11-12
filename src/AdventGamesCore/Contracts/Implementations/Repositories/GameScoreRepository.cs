using MongoDB.Driver;

namespace AdventGamesCore
{
    public class GameScoreRepository : IGameScoreRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDBService;
        private readonly IGameProfileRepository _gameProfileRepository;
        private readonly IGamePrizeRepository _gamePrizeRepository;

        #endregion

        #region Ctor

        public GameScoreRepository(
            IMongoDbService mongoDBService,
            IGameProfileRepository gameProfileRepository,
            IGamePrizeRepository gamePrizeRepository)
        {
            _mongoDBService = mongoDBService;
            _gameProfileRepository = gameProfileRepository;
            _gamePrizeRepository = gamePrizeRepository;
        }

        #endregion

        #region Methods

        #region Public

        public async Task<QueryRecordsResponse<GameScore>> GetGameScores(GetGameScoresQuery query)
        {
            return await GetGameScoresForDate(
                gameId: query.GameId,
                skip: query.PageIndex * query.PageSize,
                limit: query.PageSize,
                scoreDay: query.ScoreDay);
        }

        public async Task<QueryRecordsResponse<GameScore>> GetGameHighScores(GetGameHighScoresQuery query)
        {
            switch (query.Filter)
            {
                case HighScoreFilter.ALLTIME:
                    {
                        return await GetGameScoresForAllTime(
                            gameId: query.GameId,
                            limit: query.Limit);
                    }
                case HighScoreFilter.DATE:
                    {
                        return await GetGameScoresForDate(
                            gameId: query.GameId,
                            skip: 0,
                            limit: query.Limit,
                            scoreDay: query.FromDate.Value.ToString("dd-MMM-yyyy"));
                    }
                case HighScoreFilter.DATERANGE:
                    {
                        return await GetGameScoresForDateRange(
                            gameId: query.GameId,
                            fromDate: query.FromDate.Value,
                            toDate: query.ToDate.Value,
                            limit: query.Limit);
                    }
                default:
                    {
                        return new QueryRecordsResponse<GameScore>().BuildSuccessResponse(count: 0, records: Array.Empty<GameScore>());
                    }
            }
        }

        public async Task<ServiceResponse> SubmitGameScore(SubmitGameScoreCommand command)
        {
            GameScore gameScore;

            var currentScore = GameScore.Initialize(command);

            // get personal best score before applying current game score
            GameScore? personalBestScore = await GetPersonalBestScore(gameId: command.GameId, userId: command.User.UserId);

            // if current game score is greater than personal best score then update it
            double personalBestScoreToCommit = personalBestScore is null
                ? currentScore.Score
                : currentScore.Score > personalBestScore.Score ? currentScore.Score : personalBestScore.Score;

            await _gameProfileRepository.UpdateGameProfile(
                score: currentScore.Score,
                bestScore: personalBestScoreToCommit,
                userId: currentScore.User.UserId,
                gameId: currentScore.GameId);

            // check if a score exists for the current day
            GameScore? dailyScore = await GetTodaysScore(gameId: command.GameId, userId: command.User.UserId);

            // if no score for the day exists then insert new game score for the day
            if (dailyScore is null)
            {
                await _mongoDBService.InsertDocument(currentScore);

                gameScore = await _mongoDBService.FindById<GameScore>(currentScore.Id);
            }
            else // if current score beats existing daily high score then update
            {
                var dailyScoreToCommit = currentScore.Score > dailyScore.Score ? currentScore.Score : dailyScore.Score;

                var update = Builders<GameScore>.Update
                    .Set(x => x.Score, dailyScoreToCommit)
                    .Set(x => x.ModifiedOn, DateTime.UtcNow);

                await _mongoDBService.UpdateById(update: update, id: dailyScore.Id);

                gameScore = await _mongoDBService.FindById<GameScore>(dailyScore.Id);
            }

            // check if this score yields any game prize or not
            GamePlayResult gamePrizeResponse = await _gamePrizeRepository.GetGamePlayResult(gameScore);

            return Response.Build().BuildSuccessResponse(gamePrizeResponse);
        }

        #endregion

        #region Private

        private async Task<GameScore> GetPersonalBestScore(string gameId, string userId)
        {
            var filter = Builders<GameScore>.Filter.And(
                Builders<GameScore>.Filter.Eq(x => x.GameId, gameId),
                Builders<GameScore>.Filter.Eq(x => x.User.UserId, userId));

            var personalBestScore = await _mongoDBService.FindOne(
                filter: filter,
                sortOrder: SortOrder.Descending,
                sortFieldName: nameof(GameScore.Score));

            return personalBestScore;
        }

        private async Task<GameScore> GetTodaysScore(string gameId, string userId)
        {
            var today = DateTime.UtcNow.Date.ToString("dd-MMM-yyyy");

            var filter = Builders<GameScore>.Filter.And(
                Builders<GameScore>.Filter.Eq(x => x.GameId, gameId),
                Builders<GameScore>.Filter.Eq(x => x.User.UserId, userId),
                Builders<GameScore>.Filter.Eq(x => x.ScoreDay, today));

            var dailyScore = await _mongoDBService.FindOne(filter: filter);

            return dailyScore;
        }

        private async Task<QueryRecordsResponse<GameScore>> GetGameScoresForDateRange(string gameId, DateTime fromDate, DateTime toDate, int limit)
        {
            var filter = Builders<GameScore>.Filter.Eq(x => x.GameId, gameId);

            // player high scores are maintained day wise so get all the scores in the date range and get them as sorted by score
            filter &= Builders<GameScore>.Filter.Where(x => (x.CreatedOn >= fromDate || x.ModifiedOn >= fromDate) && (x.CreatedOn <= toDate || x.ModifiedOn <= toDate));

            var results = await _mongoDBService.GetDocuments(
                filter: filter,
                sortOrder: SortOrder.Descending,
                sortFieldName: nameof(GameScore.Score));

            // get players who played during the date range
            var usernames = results.Select(x => x.User.UserName).Distinct();

            var highScorers = new List<GameScore>();

            foreach (var username in usernames)
            {
                // as all the records are already sorted by score, the first record for each player is the highest score
                if (results.First(x => x.User.UserName == username) is GameScore playerHighScore)
                    highScorers.Add(playerHighScore);

                if (highScorers.Count >= limit)
                    break;
            }

            var count = highScorers.Count;

            return new QueryRecordsResponse<GameScore>().BuildSuccessResponse(
                count: count,
                records: highScorers.ToArray());
        }

        private async Task<QueryRecordsResponse<GameScore>> GetGameScoresForAllTime(string gameId, int limit)
        {
            var filter = Builders<GameProfile>.Filter.Eq(x => x.GameId, gameId);
            filter &= Builders<GameProfile>.Filter.Gt(x => x.PersonalBestScore, 0);

            var results = await _mongoDBService.GetDocuments(
              filter: filter,
              skip: 0,
              limit: limit,
              sortOrder: SortOrder.Descending,
              sortFieldName: nameof(GameProfile.PersonalBestScore));

            var highScorers = new List<GameScore>();

            if (results is not null)
                highScorers.AddRange(results.Select(x => GameScore.Initialize(x)));

            var count = highScorers.Count;

            return new QueryRecordsResponse<GameScore>().BuildSuccessResponse(
                count: count,
                records: highScorers.Any() ? highScorers.ToArray() : Array.Empty<GameScore>());
        }

        private async Task<QueryRecordsResponse<GameScore>> GetGameScoresForDate(string gameId, int skip, int limit, string scoreDay)
        {
            var filter = Builders<GameScore>.Filter.Eq(x => x.GameId, gameId);

            filter &= Builders<GameScore>.Filter.Eq(x => x.ScoreDay, scoreDay);

            var count = await _mongoDBService.CountDocuments(filter);

            var results = await _mongoDBService.GetDocuments(
                filter: filter,
                skip: skip,
                limit: limit,
                sortOrder: SortOrder.Descending,
                sortFieldName: nameof(GameScore.Score));

            return new QueryRecordsResponse<GameScore>().BuildSuccessResponse(
                count: results is not null ? count : 0,
                records: results is not null ? results.ToArray() : Array.Empty<GameScore>());
        }

        #endregion

        #endregion
    }
}
