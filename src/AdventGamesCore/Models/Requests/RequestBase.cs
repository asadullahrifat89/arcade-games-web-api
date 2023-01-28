using MediatR;

namespace AdventGamesCore
{
    public class RequestBase<T> : IRequest<T>
    {
        public string GameId { get; set; } = string.Empty;
    }

    public class RequestBaseDto 
    {
        public string GameId { get; set; } = string.Empty;
    }
}