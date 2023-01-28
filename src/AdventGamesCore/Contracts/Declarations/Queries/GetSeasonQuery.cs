using MediatR;

namespace AdventGamesCore
{
    public class GetSeasonQuery : IRequest<QueryRecordResponse<Season>>
    {
        public string CompanyId { get; set; } = string.Empty;
    }
}
