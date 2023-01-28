namespace AdventGamesCore
{
    public class GameSchedule : EntityBase
    {
        public string CompanyId { get; set; } = string.Empty;

        public string SeasonId { get; set; } = string.Empty;

        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        public DateTime EndTime { get; set; } = DateTime.UtcNow.AddDays(1);

        public Game Game { get; set; } = new();
    }

    public class Game
    {
        /// <summary>
        /// User defined game names.
        /// </summary>
        public CultureValue[] GameNames { get; set; } = Array.Empty<CultureValue>();

        /// <summary>
        /// User defined game descriptions.
        /// </summary>
        public CultureValue[] GameDescriptions { get; set; } = Array.Empty<CultureValue>();

        /// <summary>
        /// Maps to a hosted game url for the company.
        /// </summary>
        public string GameUrl { get; set; } = string.Empty;

        /// <summary>
        /// Maps to game ids defined in constants.
        /// </summary>
        public string GameId { get; set; } = string.Empty;
    }
}
