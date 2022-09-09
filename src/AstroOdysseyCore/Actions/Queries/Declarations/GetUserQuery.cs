using MediatR;

namespace AstroOdysseyCore
{
    public class GetUserQuery : IRequest<QueryRecordResponse<User>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
