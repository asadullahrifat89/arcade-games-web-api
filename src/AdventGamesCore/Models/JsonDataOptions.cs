namespace AdventGamesCore
{
    public class GamePrizeOptions
    {
        public GamePrize[] GamePrizes { get; set; } = Array.Empty<GamePrize>();
    }

    public class CompanyOptions
    {
        public Company[] Companies { get; set; } = Array.Empty<Company>();
    }

    public class SeasonOptions
    {
        public Season[] Seasons { get; set; } = Array.Empty<Season>();
    }

    public class GameScheduleOptions
    {
        public GameSchedule[] GameSchedules { get; set; } = Array.Empty<GameSchedule>();
    }
}
