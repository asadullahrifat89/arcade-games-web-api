using FluentValidation;

namespace AstroOdysseyCore
{
    public class GetGameProfileQueryValidator : AbstractValidator<GetGameProfileQuery>
    {
        public GetGameProfileQueryValidator()
        {
            RuleFor(x => x.GameId).NotNull().NotEmpty();
            RuleFor(x => x.UserId).NotNull().NotEmpty();
        }
    }
}
