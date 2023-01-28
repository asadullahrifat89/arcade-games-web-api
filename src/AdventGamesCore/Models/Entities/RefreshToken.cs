namespace AdventGamesCore
{
    public class RefreshToken : EntityBase
    {
        public string UserId { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;
    }
}