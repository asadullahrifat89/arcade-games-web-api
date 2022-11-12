using MediatR;

namespace AdventGamesCore
{
    public class SubmitGameScoreCommand : RequestBase<ServiceResponse>
    {
        public AttachedUser User { get; set; } = new AttachedUser();

        public double Score { get; set; } = 0;
    }
}
