namespace AdventGamesCore
{
    public class GetGameProfileQuery : RequestBase<QueryRecordResponse<GameProfile>>
    {
        public string UserId { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;
    }
}
