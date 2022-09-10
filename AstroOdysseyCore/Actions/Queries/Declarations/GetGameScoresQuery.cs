namespace AstroOdysseyCore
{
    public class GetGameScoresQuery : PagedRequestBase<QueryRecordsResponse<GameScore>>
    {
        public DateTime? Since { get; set; }
    }
}
