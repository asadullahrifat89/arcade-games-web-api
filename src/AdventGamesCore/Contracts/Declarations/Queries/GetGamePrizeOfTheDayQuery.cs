namespace AdventGamesCore
{
    public class GetGamePrizeOfTheDayQuery : RequestBase<QueryRecordResponse<GamePrizeOfTheDay>>
    {
        public int Day { get; set; } = 0;

        public string Culture { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;
    }
}
