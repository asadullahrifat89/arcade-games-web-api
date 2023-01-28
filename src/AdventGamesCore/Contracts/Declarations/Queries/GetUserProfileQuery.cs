using MediatR;

namespace AdventGamesCore
{
    public class GetUserProfileQuery : IRequest<QueryRecordResponse<UserProfile>>
    {
        public string UserId { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;
    }
}
