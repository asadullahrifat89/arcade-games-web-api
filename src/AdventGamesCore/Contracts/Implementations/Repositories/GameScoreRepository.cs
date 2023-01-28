using MongoDB.Driver;

namespace AdventGamesCore
{
    public class GameScoreRepository : IGameScoreRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDBService;
        private readonly IGameProfileRepository _gameProfileRepository;
        private readonly IGamePrizeRepository _gamePrizeRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IUserRepository _userRepository;

        #endregion

        #region Ctor

        public GameScoreRepository(
            IMongoDbService mongoDBService,
            IGameProfileRepository gameProfileRepository,
            IGamePrizeRepository gamePrizeRepository,
            ISessionRepository sessionRepository,
            IUserRepository userRepository)
        {
            _mongoDBService = mongoDBService;
            _gameProfileRepository = gameProfileRepository;
            _gamePrizeRepository = gamePrizeRepository;
            _sessionRepository = sessionRepository;
            _userRepository = userRepository;
        }

        #endregion

        #region Methods

        #region Public

        public async Task<QueryRecordsResponse<GameScore>> GetGameScores(GetGameScoresQuery query)
        {
            var (GameScores, Count) = await GetGameScoresForDate(
                gameId: query.GameId,
                skip: query.PageIndex * query.PageSize,
                limit: query.PageSize,
                scoreDay: query.ScoreDay,
                companyId: query.CompanyId);

            return new QueryRecordsResponse<GameScore>().BuildSuccessResponse(
              count: GameScores is not null ? Count : 0,
              records: GameScores is not null ? GameScores.ToArray() : Array.Empty<GameScore>());
        }

        public async Task<QueryRecordsResponse<GameHighScore>> GetGameHighScores(GetGameHighScoresQuery query)
        {
            switch (query.Filter)
            {
                case HighScoreFilter.ALLTIME:
                    {
                        return await GetGameHighScoresForAllTime(
                            gameId: query.GameId,
                            limit: query.Limit,
                            companyId: query.CompanyId);
                    }
                case HighScoreFilter.DATE:
                    {
                        return await GetGameHighScoresForDate(
                            gameId: query.GameId,
                            limit: query.Limit,
                            scoreDay: query.FromDate.Value.ToString("dd-MMM-yyyy"),
                            companyId: query.CompanyId);
                    }
                case HighScoreFilter.DATERANGE:
                    {
                        return await GetGameHighScoresForDateRange(
                            gameId: query.GameId,
                            fromDate: query.FromDate.Value,
                            toDate: query.ToDate.Value,
                            limit: query.Limit,
                            companyId: query.CompanyId);
                    }
                default:
                    {
                        return new QueryRecordsResponse<GameHighScore>().BuildSuccessResponse(count: 0, records: Array.Empty<GameHighScore>());
                    }
            }
        }

        public async Task<ServiceResponse> SubmitGameScore(SubmitGameScoreCommand command)
        {
            GameScore gameScore;

            var currentScore = GameScore.Initialize(command);

            // get personal best score before applying current game score
            GameScore? personalBestScore = await GetPersonalBestScore(
                gameId: command.GameId,
                userId: command.User.UserId,
                companyId: command.CompanyId);

            // if current game score is greater than personal best score then update it
            double personalBestScoreToCommit = personalBestScore is null
                ? currentScore.Score
                : currentScore.Score > personalBestScore.Score ? currentScore.Score : personalBestScore.Score;

            await _gameProfileRepository.UpdateGameProfile(
                score: currentScore.Score,
                bestScore: personalBestScoreToCommit,
                userId: currentScore.User.UserId,
                gameId: currentScore.GameId,
                companyId: currentScore.CompanyId);

            // check if a score exists for the current day
            GameScore? dailyScore = await GetTodaysScore(
                gameId: command.GameId,
                userId: command.User.UserId,
                companyId: command.CompanyId);

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

            // once score gets submitted session gets completed
            await _sessionRepository.CompleteSession(
                sessionId: command.SessionId,
                gameId: command.GameId);

            return Response.Build().BuildSuccessResponse(gamePrizeResponse);
        }

        public async Task BanHackers()
        {
            var filter = Builders<GameScore>.Filter.Or(
               Builders<GameScore>.Filter.Lt(x => x.Score, 0),
               Builders<GameScore>.Filter.Gt(x => x.Score, 99999),
               Builders<GameScore>.Filter.Eq(x => x.Score, double.MaxValue));

            var count = await _mongoDBService.CountDocuments(filter);

            if (count > 0)
            {
                await _mongoDBService.DeleteDocuments(filter);
            }
        }

        #endregion

        #region Private

        private async Task<GameScore> GetPersonalBestScore(string gameId, string userId, string companyId)
        {
            var filter = Builders<GameScore>.Filter.And(
                Builders<GameScore>.Filter.Eq(x => x.GameId, gameId),
                Builders<GameScore>.Filter.Eq(x => x.User.UserId, userId),
                Builders<GameScore>.Filter.Eq(x => x.CompanyId, companyId));

            var personalBestScore = await _mongoDBService.FindOne(
                filter: filter,
                sortOrder: SortOrder.Descending,
                sortFieldName: nameof(GameScore.Score));

            return personalBestScore;
        }

        private async Task<GameScore> GetTodaysScore(string gameId, string userId, string companyId)
        {
            var today = DateTime.UtcNow.Date.ToString("dd-MMM-yyyy");

            var filter = Builders<GameScore>.Filter.And(
                Builders<GameScore>.Filter.Eq(x => x.GameId, gameId),
                Builders<GameScore>.Filter.Eq(x => x.User.UserId, userId),
                Builders<GameScore>.Filter.Eq(x => x.ScoreDay, today),
                Builders<GameScore>.Filter.Eq(x => x.CompanyId, companyId));

            var dailyScore = await _mongoDBService.FindOne(filter: filter);

            return dailyScore;
        }

        private async Task<QueryRecordsResponse<GameHighScore>> GetGameHighScoresForDate(string gameId, int limit, string scoreDay, string companyId)
        {
            var (GameScores, Count) = await GetGameScoresForDate(
                gameId: gameId,
                skip: 0,
                limit: limit,
                scoreDay: scoreDay,
                companyId: companyId);

            if (GameScores is null || GameScores.Count == 0)
                return new QueryRecordsResponse<GameHighScore>().BuildSuccessResponse(0, Array.Empty<GameHighScore>());

            var gameScores = GameScores;
            var count = Count;

            var userIds = gameScores.Select(x => x.User.UserId).ToArray();

            var users = await _userRepository.GetUsers(
                userIds: userIds,
                companyId: companyId);

            List<GameHighScore> gameHighScores = new();

            foreach (var gameScore in gameScores)
            {
                var user = users.First(x => x.Id == gameScore.User.UserId);
                gameHighScores.Add(GameHighScore.Initialize(gameScore, user));
            }

            return new QueryRecordsResponse<GameHighScore>().BuildSuccessResponse(
                count: count,
                records: gameHighScores.ToArray());
        }

        private async Task<QueryRecordsResponse<GameHighScore>> GetGameHighScoresForDateRange(string gameId, DateTime fromDate, DateTime toDate, int limit, string companyId)
        {
            var (GameScores, Count) = await GetGameScoresForDateRange(
                gameId: gameId,
                fromDate: fromDate,
                toDate: toDate,
                limit: limit,
                companyId: companyId);

            if (GameScores is null || GameScores.Count == 0)
                return new QueryRecordsResponse<GameHighScore>().BuildSuccessResponse(0, Array.Empty<GameHighScore>());

            var gameScores = GameScores;
            var count = Count;

            var userIds = gameScores.Select(x => x.User.UserId).ToArray();

            var users = await _userRepository.GetUsers(
                userIds: userIds,
                companyId: companyId);

            List<GameHighScore> gameHighScores = new();

            foreach (var gameScore in gameScores)
            {
                var user = users.First(x => x.Id == gameScore.User.UserId);
                gameHighScores.Add(GameHighScore.Initialize(gameScore, user));
            }

            return new QueryRecordsResponse<GameHighScore>().BuildSuccessResponse(
                count: count,
                records: gameHighScores.ToArray());
        }

        private async Task<(List<GameScore> GameScores, long Count)> GetGameScoresForDateRange(string gameId, DateTime fromDate, DateTime toDate, int limit, string companyId)
        {
            var filter = Builders<GameScore>.Filter.And(
                Builders<GameScore>.Filter.Eq(x => x.GameId, gameId),
                Builders<GameScore>.Filter.Eq(x => x.CompanyId, companyId),
                Builders<GameScore>.Filter.Gt(x => x.Score, 0),
                Builders<GameScore>.Filter.Lt(x => x.Score, double.MaxValue),
                Builders<GameScore>.Filter.Lt(x => x.Score, 99999),
                Builders<GameScore>.Filter.Where(x => (x.CreatedOn >= fromDate || x.ModifiedOn >= fromDate) && (x.CreatedOn <= toDate || x.ModifiedOn <= toDate)));

            // get all game scores which fall between the date range
            var gameScores = await _mongoDBService.GetDocuments(
                filter: filter,
                sortOrder: SortOrder.Descending,
                sortFieldName: nameof(GameScore.Score));

            if (gameScores is null || gameScores.Count == 0)
                return (new List<GameScore>(), 0);

            var usernames = gameScores.Select(x => x.User.UserName).Distinct();

            var highScorers = new List<GameScore>();

            // take the first score as it is the highest score
            foreach (var username in usernames)
            {
                if (gameScores.First(x => x.User.UserName == username) is GameScore playerHighScore)
                    highScorers.Add(playerHighScore);

                if (highScorers.Count >= limit)
                    break;
            }

            var count = highScorers.Count;

            return (highScorers, count);
        }

        private async Task<(List<GameScore> GameScores, long Count)> GetGameScoresForDate(string gameId, int skip, int limit, string scoreDay, string companyId)
        {
            var filter = Builders<GameScore>.Filter.And(
                Builders<GameScore>.Filter.Eq(x => x.GameId, gameId),
                Builders<GameScore>.Filter.Eq(x => x.CompanyId, companyId),
                Builders<GameScore>.Filter.Gt(x => x.Score, 0),
                Builders<GameScore>.Filter.Lt(x => x.Score, double.MaxValue),
                Builders<GameScore>.Filter.Lt(x => x.Score, 99999),
                Builders<GameScore>.Filter.Eq(x => x.ScoreDay, scoreDay));

            var count = await _mongoDBService.CountDocuments(filter);

            var results = await _mongoDBService.GetDocuments(
                filter: filter,
                skip: skip,
                limit: limit,
                sortOrder: SortOrder.Descending,
                sortFieldName: nameof(GameScore.Score));

            return (results, count);
        }

        private async Task<QueryRecordsResponse<GameHighScore>> GetGameHighScoresForAllTime(string gameId, int limit, string companyId)
        {
            var filter = Builders<GameProfile>.Filter.And(
                Builders<GameProfile>.Filter.Eq(x => x.GameId, gameId),
                Builders<GameProfile>.Filter.Eq(x => x.CompanyId, companyId),
                Builders<GameProfile>.Filter.Gt(x => x.PersonalBestScore, 0),
                Builders<GameProfile>.Filter.Lt(x => x.PersonalBestScore, double.MaxValue),
                Builders<GameProfile>.Filter.Lt(x => x.PersonalBestScore, 99999));

            var gameProfiles = await _mongoDBService.GetDocuments(
              filter: filter,
              skip: 0,
              limit: limit,
              sortOrder: SortOrder.Descending,
              sortFieldName: nameof(GameProfile.PersonalBestScore));

            if (gameProfiles is null || gameProfiles.Count == 0)
                return new QueryRecordsResponse<GameHighScore>().BuildSuccessResponse(0, Array.Empty<GameHighScore>());

            var count = gameProfiles.Count;

            var userIds = gameProfiles.Select(x => x.User.UserId).ToArray();

            var users = await _userRepository.GetUsers(
                userIds: userIds,
                companyId: companyId);

            List<GameHighScore> gameHighScores = new();

            foreach (var gameProfile in gameProfiles)
            {
                var user = users.First(x => x.Id == gameProfile.User.UserId);
                gameHighScores.Add(GameHighScore.Initialize(gameProfile, user));
            }

            return new QueryRecordsResponse<GameHighScore>().BuildSuccessResponse(
                count: count,
                records: gameHighScores.ToArray());
        }

        #endregion

        #endregion
    }
}
