using MediatR;

namespace SpaceShooterCore
{
    public class GetUserQuery : IRequest<QueryRecordResponse<User>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
