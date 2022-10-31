namespace SpaceShooterCore
{
    public class GetGameScoresQuery : PagedRequestBase<QueryRecordsResponse<GameScore>>
    {
        public string ScoreDay { get; set; } = DateTime.UtcNow.Date.ToString("dd-MMM-yyyy");
    }
}
