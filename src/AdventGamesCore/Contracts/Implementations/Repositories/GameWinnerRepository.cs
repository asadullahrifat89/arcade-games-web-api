using AdventGamesCore.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;

namespace AdventGamesCore
{
    public class GameWinnerRepository : IGameWinnerRepository
    {
        #region Fields

        private readonly IGameScoreRepository _gameScoreRepository;
        private readonly IGamePrizeRepository _gamePrizeRepository;
        private readonly IUserRepository _userRepository;

        #endregion

        #region Ctor

        public GameWinnerRepository(
            IGameScoreRepository gameScoreRepository,
            IGamePrizeRepository gamePrizeRepository,
            IUserRepository userRepository)
        {
            _gameScoreRepository = gameScoreRepository;
            _gamePrizeRepository = gamePrizeRepository;
            _userRepository = userRepository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordsResponse<GameWinner>> GetGameWinners(GetGameWinnersQuery query)
        {
            List<GameWinner> gameWinners = new List<GameWinner>();

            var gameHighScoresResponse = await _gameScoreRepository.GetGameHighScores(new GetGameHighScoresQuery()
            {
                GameId = query.GameId,
                Filter = query.Filter,
                FromDate = query.FromDate,
                ToDate = query.ToDate,
                Limit = query.Limit,
            });

            if (gameHighScoresResponse is not null && gameHighScoresResponse.IsSuccess && gameHighScoresResponse.Result.Count > 0)
            {
                var gameScores = gameHighScoresResponse.Result.Records;

                var userIds = gameScores.Select(x => x.User.UserId).ToArray();

                var users = await _userRepository.GetUsers(userIds);

                foreach (var gameScore in gameScores)
                {
                    var gamePlayResult = await _gamePrizeRepository.GetGamePlayResult(gameScore);

                    var user = users.FirstOrDefault(x => x.Id == gameScore.User.UserId);

                    GameWinner gameWinner = new()
                    {
                        City = user.City,
                        FullName = user.FullName,
                        UserEmail = user.Email,
                        UserName = user.UserName,                        
                        Score = gameScore.Score,
                        ScoreDay = gameScore.ScoreDay,
                        PrizeName = gamePlayResult.PrizeName,
                        PrizeDescriptions = gamePlayResult.PrizeDescriptions,
                    };

                    gameWinners.Add(gameWinner);
                }
            }

            var count = gameWinners.Count;

            return new QueryRecordsResponse<GameWinner>().BuildSuccessResponse(
                count: count,
                records: gameWinners.ToArray());
        }

        #region Private      

        #endregion

        #endregion
    }
}
