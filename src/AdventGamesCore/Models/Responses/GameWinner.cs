namespace AdventGamesCore
{
    public class GameWinner
    {
        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public double Score { get; set; } = 0;

        public string ScoreDay { get; set; } = string.Empty;

        public string PrizeName { get; set; } = string.Empty;

        public CultureValue[] PrizeDescriptions { get; set; } = Array.Empty<CultureValue>();
    }
}
