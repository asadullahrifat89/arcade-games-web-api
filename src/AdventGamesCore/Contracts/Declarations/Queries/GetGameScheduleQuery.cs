using MediatR;

namespace AdventGamesCore
{
    public class GetGameScheduleQuery : IRequest<QueryRecordResponse<GameSchedule>>
    {
        public string CompanyId { get; set; } = string.Empty;

        public string SeasonId { get; set; } = string.Empty;
    }
}
