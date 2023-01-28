namespace AdventGamesCore
{
    public class GetGameProfilesQuery : PagedRequestBase<QueryRecordsResponse<GameProfile>>
    {
        public string CompanyId { get; set; } = string.Empty;
    }
}
