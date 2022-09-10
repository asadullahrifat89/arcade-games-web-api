using MediatR;

namespace AstroOdysseyCore
{
    public class RequestBase<T> : IRequest<T>
    {
        public string GameId { get; set; } = string.Empty;
    }
}