using MediatR;

namespace AdventGamesCore
{
    public class GetCompanyQuery : IRequest<QueryRecordResponse<Company>>
    {
        public string CompanyId { get; set; } = string.Empty;
    }
}
