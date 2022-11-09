namespace AdventGamesCore
{
    public class GetGameHighScoresQuery : RequestBase<QueryRecordsResponse<GameScore>>
    {
        public HighScoreFilter Filter { get; set; }

        public int Limit { get; set; } = 0;

        public DateTime? FromDate { get; set; } = DateTime.UtcNow;

        public DateTime? ToDate { get; set; } = DateTime.UtcNow;
    }

    public enum HighScoreFilter
    {
        ALLTIME,
        DATE,
        DATERANGE,
    }
}
