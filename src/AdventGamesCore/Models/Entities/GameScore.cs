namespace AdventGamesCore
{
    public class GameScore : EntityBase
    {
        public AttachedUser User { get; set; } = new AttachedUser();

        public double Score { get; set; } = 0;

        public string GameId { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;

        public string ScoreDay { get; set; } = DateTime.UtcNow.Date.ToString("dd-MMM-yyyy");

        public static GameScore Initialize(SubmitGameScoreCommand command)
        {
            return new GameScore()
            {
                Score = command.Score,
                GameId = command.GameId,
                User = command.User,
                CompanyId = command.CompanyId,
                ScoreDay = DateTime.UtcNow.Date.ToString("dd-MMM-yyyy")
            };
        }

        public static GameScore Initialize(GameHighScore gameHighScore)
        {
            return new GameScore()
            {
                Score = gameHighScore.Score,
                GameId = gameHighScore.GameId,
                User = new AttachedUser() { UserId = gameHighScore.UserId, UserName = gameHighScore.UserName },
                CompanyId = gameHighScore.CompanyId,
                ScoreDay = gameHighScore.ScoreDay
            };
        }
    }
}
