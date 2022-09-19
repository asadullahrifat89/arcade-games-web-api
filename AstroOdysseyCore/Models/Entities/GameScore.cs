namespace AstroOdysseyCore
{
    public class GameScore : EntityBase
    {
        public AttachedUser User { get; set; } = new AttachedUser();

        public double Score { get; set; } = 0;

        public string GameId { get; set; } = string.Empty;

        public DateTime ScoreDay { get; set; } = DateTime.UtcNow.Date;

        public static GameScore Initialize(SubmitGameScoreCommand command)
        {
            return new GameScore()
            {
                Score = command.Score,
                GameId = command.GameId,
                User = command.User,
            };
        }
    }
}
