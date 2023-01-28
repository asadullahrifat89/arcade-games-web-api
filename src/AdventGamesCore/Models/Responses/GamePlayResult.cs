namespace AdventGamesCore
{
    public class GamePlayResult
    {
        public string GameId { get; set; } = string.Empty;

        public string PrizeName { get; set; } = string.Empty;

        public CultureValue[] PrizeDescriptions { get; set; } = Array.Empty<CultureValue>();

        public CultureValue[] WinningDescriptions { get; set; } = Array.Empty<CultureValue>();

        public CultureValue[] PrizeUrls { get; set; } = Array.Empty<CultureValue>();

        public static GamePlayResult Initialize(GamePrize gamePrize, CultureValue[] winningDescriptions)
        {
            return new GamePlayResult
            {
                GameId = gamePrize.GameId,
                PrizeName = gamePrize.Name,
                PrizeDescriptions = gamePrize.PrizeDescriptions,
                WinningDescriptions = winningDescriptions,
                PrizeUrls = gamePrize.PrizeUrls,
            };
        }
    }
}
