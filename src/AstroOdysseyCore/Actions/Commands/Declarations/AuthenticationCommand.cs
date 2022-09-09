using MediatR;

namespace AstroOdysseyCore
{
    public class AuthenticationCommand : IRequest<QueryRecordResponse<AuthToken>>
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public class AuthToken
    {
        public string Token { get; set; } = string.Empty;

        public DateTime LifeTime { get; set; }
    }
}