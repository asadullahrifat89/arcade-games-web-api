using MediatR;

namespace AstroOdysseyCore
{
    public class AuthenticationCommand : IRequest<ServiceResponse>
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}