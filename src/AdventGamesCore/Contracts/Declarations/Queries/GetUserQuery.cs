using MediatR;

namespace AdventGamesCore
{
    public class GetUserQuery : IRequest<QueryRecordResponse<User>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
