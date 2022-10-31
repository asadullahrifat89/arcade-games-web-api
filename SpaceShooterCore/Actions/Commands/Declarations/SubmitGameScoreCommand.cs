using MediatR;

namespace SpaceShooterCore
{
    public class SubmitGameScoreCommand : IRequest<ServiceResponse>
    {
        public AttachedUser User { get; set; } = new AttachedUser();

        public double Score { get; set; } = 0;

        public string GameId { get; set; } = string.Empty;
    }
}
