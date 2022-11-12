using MediatR;

namespace AdventGamesCore
{
    /// <summary>
    /// Generates a session for a user and game and returns encrypted session.
    /// </summary>
    public class GenerateSessionCommand : RequestBase<ServiceResponse>
    {
        public string UserId { get; set; } = string.Empty;
    }
}