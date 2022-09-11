using MediatR;

namespace AstroOdysseyCore
{
    public class ValidateSessionCommand : IRequest<ServiceResponse> 
    {
        public string SessionId { get; set; } = string.Empty;

        public string GameId { get; set; } = string.Empty;
    }
}