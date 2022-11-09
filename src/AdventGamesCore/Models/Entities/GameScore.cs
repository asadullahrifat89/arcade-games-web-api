namespace AdventGamesCore
{
    public class GameScore : EntityBase
    {
        public AttachedUser User { get; set; } = new AttachedUser();

        public double Score { get; set; } = 0;

        public string GameId { get; set; } = string.Empty;

        public string ScoreDay { get; set; } = DateTime.UtcNow.Date.ToString("dd-MMM-yyyy");

        public static GameScore Initialize(SubmitGameScoreCommand command)
        {
            return new GameScore()
            {
                Score = command.Score,
                GameId = command.GameId,
                User = command.User,
            };
        }

        public static GameScore Initialize(GameProfile gameProfile)
        {
            return new GameScore()
            {
                Score = gameProfile.PersonalBestScore,
                GameId = gameProfile.GameId,
                User = gameProfile.User,
                ScoreDay = gameProfile.ModifiedOn is not null ? gameProfile.ModifiedOn.Value.ToString("dd-MMM-yyyy") : gameProfile.CreatedOn.ToString("dd-MMM-yyyy")
            };
        }
    }
}
