using MediatR;

namespace SpaceShooterCore
{
    public class SignupCommand : IRequest<ServiceResponse>
    {
        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string GameId { get; set; } = string.Empty;

        public Dictionary<string, string> MetaData { get; set; } = new Dictionary<string, string>();
    }
}
