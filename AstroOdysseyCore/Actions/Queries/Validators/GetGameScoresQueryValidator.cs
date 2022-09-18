using FluentValidation;

namespace AstroOdysseyCore
{
    public class GetGameScoresQueryValidator : AbstractValidator<GetGameScoresQuery>
    {
        public GetGameScoresQueryValidator()
        {
            RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PageSize).GreaterThan(0);
            RuleFor(x => x.GameId).NotNull().NotEmpty();
            RuleFor(x => x.ScoreDay).Must(scoreDay => scoreDay <= DateTime.UtcNow).WithMessage("Score day can't be before today.");
        }
    }
}
