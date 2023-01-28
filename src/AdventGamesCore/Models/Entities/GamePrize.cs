namespace AdventGamesCore
{
    public class GamePrize : EntityBase
    {
        public string GameId { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;

        public string SeasonId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int[] Days { get; set; } = Array.Empty<int>();

        public CultureValue[] PrizeDescriptions { get; set; } = Array.Empty<CultureValue>();

        public WinningCriteria WinningCriteria { get; set; } = new();

        public CultureValue[] PrizeUrls { get; set; } = Array.Empty<CultureValue>();
    }

    public class WinningCriteria
    {
        public WinningCriteriaType CriteriaType { get; set; } = WinningCriteriaType.DailyHighScore;

        public double ScoreThreshold { get; set; }

        public CultureValue[] CriteriaDescriptions { get; set; } = Array.Empty<CultureValue>();

        public CultureValue[] WinningDescriptions { get; set; } = Array.Empty<CultureValue>();

        public CultureValue[] MotivationDescriptions { get; set; } = Array.Empty<CultureValue>();
    }

    public enum WinningCriteriaType
    {
        DailyHighScore,
        ScoreThreshold,
    }
}
