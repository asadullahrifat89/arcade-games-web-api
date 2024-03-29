﻿namespace AdventGamesCore
{
    public class GetGameScoresQuery : PagedRequestBase<QueryRecordsResponse<GameScore>>
    {
        public string ScoreDay { get; set; } = DateTime.UtcNow.Date.ToString("dd-MMM-yyyy");

        public string CompanyId { get; set; } = string.Empty;
    }
}
