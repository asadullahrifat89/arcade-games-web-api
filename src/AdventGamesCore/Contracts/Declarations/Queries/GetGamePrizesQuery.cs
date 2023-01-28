namespace AdventGamesCore
{
    public class GetGamePrizesQuery : PagedRequestBase<QueryRecordsResponse<GamePrize>>
    {
        public string CompanyId { get; set; } = string.Empty;

        public string? SearchTerm { get; set; } = null;

        public int? Day { get; set; } = null;

        public string? Culture { get; set; }
    }
}
