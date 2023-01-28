using MediatR;

namespace AdventGamesCore
{
    public class AuthenticateCommand : IRequest<ServiceResponse>
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;
    }
}