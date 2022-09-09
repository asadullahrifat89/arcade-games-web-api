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
            RuleFor(x => x.Since).Must(since => since <= DateTime.UtcNow).When(x => x.Since is not null);
        }
    }
}
