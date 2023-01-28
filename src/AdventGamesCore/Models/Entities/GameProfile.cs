namespace AdventGamesCore
{
    public class GameProfile : EntityBase
    {
        public AttachedUser User { get; set; } = new AttachedUser();

        public double PersonalBestScore { get; set; } = 0;

        public double LastGameScore { get; set; } = 0;

        public string GameId { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;

        public static GameProfile Initialize(SignupCommand command, string userId, string gameId)
        {
            return new GameProfile()
            {
                GameId = gameId,
                LastGameScore = 0,
                PersonalBestScore = 0,
                CompanyId = command.CompanyId,
                User = new AttachedUser()
                {
                    UserId = userId,
                    UserName = command.UserName
                },
            };
        }
    }
}
