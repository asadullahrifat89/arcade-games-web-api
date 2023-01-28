using AdventGamesCore.Extensions;
using FluentValidation;

namespace AdventGamesCore
{
    public class GetGameScheduleQueryValidator : AbstractValidator<GetGameScheduleQuery>
    {
        private readonly ICompanyRepository _companyRepository;

        public GetGameScheduleQueryValidator(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;

            RuleFor(x => x.SeasonId).NotNull().NotEmpty();

            RuleFor(x => x.CompanyId).NotNull().NotEmpty();
            RuleFor(x => x.CompanyId).MustAsync(BeAnExistingCompany).WithMessage("Company doesn't exist.").When(x => !x.CompanyId.IsNullOrBlank());
        }

        private async Task<bool> BeAnExistingCompany(string userName, CancellationToken arg2)
        {
            return await _companyRepository.BeAnExistingCompany(userName);
        }
    }
}
