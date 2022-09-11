using MediatR;

namespace AstroOdysseyCore
{
    /// <summary>
    /// Checks if a session exists and returns an auth token if it does.
    /// </summary>
    public class ValidateSessionCommand : IRequest<ServiceResponse> 
    {
        public string SessionId { get; set; } = string.Empty;

        public string GameId { get; set; } = string.Empty;
    }
}