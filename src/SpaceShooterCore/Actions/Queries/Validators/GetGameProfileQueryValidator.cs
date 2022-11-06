using FluentValidation;

namespace SpaceShooterCore
{
    public class GetGameProfileQueryValidator : AbstractValidator<GetGameProfileQuery>
    {
        public GetGameProfileQueryValidator()
        {
            RuleFor(x => x.GameId).NotNull().NotEmpty();
            RuleFor(x => x.GameId).Must(x => Constants.GAME_IDS.Contains(x)).WithMessage("Invalid game id.");

            RuleFor(x => x.UserId).NotNull().NotEmpty();
        }
    }
}
