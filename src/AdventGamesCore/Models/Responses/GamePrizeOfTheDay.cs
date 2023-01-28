namespace AdventGamesCore
{
    public class GamePrizeOfTheDay
    {
        public string GameId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public CultureValue[] PrizeDescriptions { get; set; } = Array.Empty<CultureValue>();

        public WinningCriteria WinningCriteria { get; set; } = new();

        public static GamePrizeOfTheDay Initialize(GamePrize gamePrize)
        {
            return new GamePrizeOfTheDay()
            {
                GameId = gamePrize.GameId,
                Name = gamePrize.Name,
                PrizeDescriptions = gamePrize.PrizeDescriptions,
                WinningCriteria = gamePrize.WinningCriteria,
            };
        }
    }
}
