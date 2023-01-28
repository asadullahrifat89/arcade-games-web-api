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
            List<GameWinner> gameWinners = new();

            var gameHighScoresResponse = await _gameScoreRepository.GetGameHighScores(new GetGameHighScoresQuery()
            {
                GameId = query.GameId,
                Filter = query.Filter,
                FromDate = query.FromDate,
                ToDate = query.ToDate,
                Limit = query.Limit,
                CompanyId = query.CompanyId,
            });

            if (gameHighScoresResponse is not null && gameHighScoresResponse.IsSuccess && gameHighScoresResponse.Result.Count > 0)
            {
                var gameHighScores = gameHighScoresResponse.Result.Records;

                foreach (var gameHighScore in gameHighScores)
                {
                    var gamePlayResult = await _gamePrizeRepository.GetGamePlayResult(GameScore.Initialize(gameHighScore));

                    GameWinner gameWinner = GameWinner.Initialize(gameHighScore: gameHighScore, gamePlayResult: gamePlayResult);

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
