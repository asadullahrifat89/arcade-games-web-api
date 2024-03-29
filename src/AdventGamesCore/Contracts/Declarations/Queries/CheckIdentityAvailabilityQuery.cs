﻿namespace AdventGamesCore
{
    public class CheckIdentityAvailabilityQuery : RequestBase<QueryRecordResponse<bool>>
    {
        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;
    }
}
