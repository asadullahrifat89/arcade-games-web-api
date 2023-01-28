namespace AdventGamesCore
{
    public class GameHighScore
    {
        public string FullName { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public double Score { get; set; } = 0;

        public string ScoreDay { get; set; } = string.Empty;

        public string GameId { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;

        public static GameHighScore Initialize(GameScore gameScore, User user)
        {
            return new GameHighScore()
            {
                Score = gameScore.Score,
                City = user.City,
                CompanyId = gameScore.CompanyId,
                FullName = user.FullName,
                GameId = gameScore.GameId,
                ScoreDay = gameScore.ScoreDay,
                UserEmail = user.Email,
                UserId = user.Id,
                UserName = user.UserName,
            };
        }

        public static GameHighScore Initialize(GameProfile gameProfile, User user)
        {
            return new GameHighScore()
            {
                Score = gameProfile.PersonalBestScore,
                City = user.City,
                CompanyId = gameProfile.CompanyId,
                FullName = user.FullName,
                GameId = gameProfile.GameId,
                ScoreDay = gameProfile.ModifiedOn is null ? gameProfile.CreatedOn.ToString("dd-MMM-yyyy") : gameProfile.ModifiedOn.Value.ToString("dd-MMM-yyyy"),
                UserEmail = user.Email,
                UserId = user.Id,
                UserName = user.UserName,
            };
        }
    }
}
