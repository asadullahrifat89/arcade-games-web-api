namespace AstroOdysseyCore
{
    public class GetGameScoresQuery : PagedRequestBase<QueryRecordsResponse<GameScore>>
    {
        public DateTime ScoreDay { get; set; }
    }
}
