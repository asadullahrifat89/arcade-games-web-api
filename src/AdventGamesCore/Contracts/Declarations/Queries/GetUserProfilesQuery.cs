namespace AdventGamesCore
{
    public class GetUserProfilesQuery : PagedRequestBase<QueryRecordsResponse<UserProfile>>
    {
        public string CompanyId { get; set; } = string.Empty;
    }
}
