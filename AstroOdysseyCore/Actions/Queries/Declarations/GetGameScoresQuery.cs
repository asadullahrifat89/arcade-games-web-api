namespace AstroOdysseyCore
{
    public class GetGameScoresQuery : PagedRequestBase<QueryRecordsResponse<GameScore>>
    {
        public string ScoreDay { get; set; } = DateTime.Now.Date.ToString("dd-MMM-yyyy");
    }
}
