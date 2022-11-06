using MediatR;

namespace SpaceShooterCore
{
    public class AuthenticateCommand : IRequest<ServiceResponse>
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}