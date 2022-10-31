using MediatR;

namespace SpaceShooterCore
{
    public class RequestBase<T> : IRequest<T>
    {
        public string GameId { get; set; } = string.Empty;
    }
}