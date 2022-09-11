using MediatR;

namespace AstroOdysseyCore
{
    /// <summary>
    /// Generates a session for a user and returns sessionid.
    /// </summary>
    public class GenerateSessionCommand : IRequest<ServiceResponse>
    {
        public string GameId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
    }
}