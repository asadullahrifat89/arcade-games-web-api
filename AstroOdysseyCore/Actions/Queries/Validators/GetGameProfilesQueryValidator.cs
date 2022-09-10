using FluentValidation;

namespace AstroOdysseyCore
{
    public class GetGameProfilesQueryValidator : AbstractValidator<GetGameProfilesQuery>
    {
        public GetGameProfilesQueryValidator()
        {
            RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PageSize).GreaterThan(0);
            RuleFor(x => x.GameId).NotNull().NotEmpty();
        }
    }
}
