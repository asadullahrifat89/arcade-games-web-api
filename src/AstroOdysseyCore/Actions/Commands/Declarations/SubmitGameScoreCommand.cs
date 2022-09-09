using MediatR;

namespace AstroOdysseyCore
{
    public class SubmitGameScoreCommand : IRequest<ActionCommandResponse>
    {
        public AttachedUser User { get; set; } = new AttachedUser();

        public double Score { get; set; } = 0;

        public string GameId { get; set; } = string.Empty;
    }
}
