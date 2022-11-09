namespace AdventGamesCore
{
    public class GamePrize : EntityBase
    {
        public int Day { get; set; } = 1;

        public CultureValue[] Descriptions { get; set; } = Array.Empty<CultureValue>();

        public PrizeWinningCriteria PrizeWinningCriteria { get; set; } = new();
    }

    public class PrizeWinningCriteria
    {
        public CultureValue[] Descriptions { get; set; } = Array.Empty<CultureValue>();

        public double ScoreThreshold { get; set; }

        public WinningCriteria Criteria { get; set; } = WinningCriteria.None;
    }

    public enum WinningCriteria
    {
        None,
        ScoreThreshold,
        HighScore
    }
}
