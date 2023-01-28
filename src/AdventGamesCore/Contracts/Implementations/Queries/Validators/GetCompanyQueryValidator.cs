using FluentValidation;

namespace AdventGamesCore
{
    public class GetCompanyQueryValidator : AbstractValidator<GetCompanyQuery>
    {
        public GetCompanyQueryValidator()
        {
            RuleFor(x => x.CompanyId).NotNull().NotEmpty();
        }
    }
}
