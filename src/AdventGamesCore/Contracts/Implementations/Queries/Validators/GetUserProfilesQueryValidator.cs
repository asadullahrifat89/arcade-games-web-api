using AdventGamesCore.Extensions;
using FluentValidation;

namespace AdventGamesCore
{
    public class GetUserProfilesQueryValidator : AbstractValidator<GetUserProfilesQuery>
    {
        private readonly ICompanyRepository _companyRepository;

        public GetUserProfilesQueryValidator(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;

            RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PageSize).GreaterThan(0);            

            RuleFor(x => x.CompanyId).NotNull().NotEmpty();
            RuleFor(x => x.CompanyId).MustAsync(BeAnExistingCompany).WithMessage("Company doesn't exist.").When(x => !x.CompanyId.IsNullOrBlank());
        }

        private async Task<bool> BeAnExistingCompany(string userName, CancellationToken arg2)
        {
            return await _companyRepository.BeAnExistingCompany(userName);
        }
    }
}
