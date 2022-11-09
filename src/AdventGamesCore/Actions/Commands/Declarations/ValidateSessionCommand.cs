using MediatR;

namespace AdventGamesCore
{
    /// <summary>
    /// Checks if a session exists and returns an auth token if it does.
    /// </summary>
    public class ValidateSessionCommand : RequestBase<ServiceResponse> 
    {
        public string SessionId { get; set; } = string.Empty;
    }
}