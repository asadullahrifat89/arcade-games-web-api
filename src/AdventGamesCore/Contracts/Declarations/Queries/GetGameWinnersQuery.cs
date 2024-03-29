﻿namespace AdventGamesCore
{
    public class GetGameWinnersQuery : RequestBase<QueryRecordsResponse<GameWinner>>
    {
        public HighScoreFilter Filter { get; set; }

        public int Limit { get; set; } = 0;

        public string CompanyId { get; set; } = string.Empty;

        public DateTime? FromDate { get; set; } = DateTime.UtcNow;

        public DateTime? ToDate { get; set; } = DateTime.UtcNow;
    }
}
