using AdventGamesCore.Extensions;
using FluentValidation;

namespace AdventGamesCore
{
    public class GetGameProfilesQueryValidator : AbstractValidator<GetGameProfilesQuery>
    {
        private readonly ICompanyRepository _companyRepository;

        public GetGameProfilesQueryValidator(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;

            RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PageSize).GreaterThan(0);
            RuleFor(x => x.GameId).NotNull().NotEmpty();
            RuleFor(x => x.GameId).Must(x => Constants.GAME_IDS.Contains(x)).WithMessage("Invalid game id.");

            RuleFor(x => x.CompanyId).NotNull().NotEmpty();
            RuleFor(x => x.CompanyId).MustAsync(BeAnExistingCompany).WithMessage("Company doesn't exist.").When(x => !x.CompanyId.IsNullOrBlank());
        }

        private async Task<bool> BeAnExistingCompany(string userName, CancellationToken arg2)
        {
            return await _companyRepository.BeAnExistingCompany(userName);
        }
    }
}
