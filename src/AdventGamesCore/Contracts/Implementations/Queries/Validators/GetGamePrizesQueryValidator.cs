using FluentValidation;

namespace AdventGamesCore
{
    public class GetGamePrizesQueryValidator : AbstractValidator<GetGamePrizesQuery>
    {
        public GetGamePrizesQueryValidator()
        {
            RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PageSize).GreaterThan(0);
            RuleFor(x => x.GameId).NotNull().NotEmpty();
            RuleFor(x => x.GameId).Must(x => Constants.GAME_IDS.Contains(x)).WithMessage("Invalid game id.");
        }
    }
}
