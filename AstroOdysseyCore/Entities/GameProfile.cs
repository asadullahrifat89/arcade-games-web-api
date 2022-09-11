using AstroOdysseyCore.Extensions;

namespace AstroOdysseyCore
{
    public class GameProfile : EntityBase
    {
        public AttachedUser User { get; set; } = new AttachedUser();

        public double PersonalBestScore { get; set; } = 0;

        public double LastGameScore { get; set; } = 0;

        public string GameId { get; set; } = string.Empty;

        public static GameProfile Initialize(SignupCommand command, string userId)
        {
            return new GameProfile()
            {
                GameId = command.GameId,
                LastGameScore = 0,
                PersonalBestScore = 0,
                User = new AttachedUser()
                {
                    UserId = userId,
                    UserName = command.UserName,
                    UserEmail = command.Email,
                },
            };
        }
    }
}
