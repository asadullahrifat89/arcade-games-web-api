using FluentValidation;

namespace AdventGamesCore
{
    public class GetGamePrizeQueryValidator : AbstractValidator<GetGamePrizeQuery>
    {
        public GetGamePrizeQueryValidator()
        {
            RuleFor(x => x.GameId).NotNull().NotEmpty();
            RuleFor(x => x.GameId).Must(x => Constants.GAME_IDS.Contains(x)).WithMessage("Invalid game id.");

            RuleFor(x => x.Day).GreaterThan(0);
            RuleFor(x => x.Culture).NotNull().NotEmpty();
        }
    }
}
