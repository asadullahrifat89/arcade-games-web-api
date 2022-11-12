namespace AdventGamesCore
{
    public class GetGamePrizeQuery : RequestBase<QueryRecordResponse<GamePrize>>
    {
        public int Day { get; set; } = 0;

        public string Culture { get; set; } = string.Empty;
    }
}
