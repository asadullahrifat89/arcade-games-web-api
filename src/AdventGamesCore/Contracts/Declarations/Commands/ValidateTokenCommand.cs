using MediatR;

namespace AdventGamesCore
{
    /// <summary>
    /// Checks if a session exists and returns an auth token if it does.
    /// </summary>
    public class ValidateTokenCommand : IRequest<ServiceResponse> 
    {
        public string RefreshToken { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;
    }
}