namespace AstroOdysseyCore
{
    public class UserGameProfile : EntityBase
    {
        public AttachedUser User { get; set; } = new AttachedUser();

        public double PersonalBestScore { get; set; } = 0;

        public double LastGameScore { get; set; } = 0;

        public string GameId { get; set; } = string.Empty;
    }
}
