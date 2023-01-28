using AdventGamesCore.Extensions;
using FluentValidation;

namespace AdventGamesCore
{
    public class CheckIdentityAvailabilityQueryValidator : AbstractValidator<CheckIdentityAvailabilityQuery>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;

        public CheckIdentityAvailabilityQueryValidator(
            IUserRepository userRepository,
            ICompanyRepository companyRepository)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;

            RuleFor(x => x.UserName).NotNull().NotEmpty();
            RuleFor(x => x).MustAsync(NotBeAnExistingUserName).WithMessage("Username already exists.");

            RuleFor(x => x.Email).NotNull().NotEmpty();
            RuleFor(x => x).MustAsync(NotBeAnExistingUserEmail).WithMessage("Email already exists.");

            RuleFor(x => x.GameId).NotNull().NotEmpty();
            RuleFor(x => x.GameId).Must(x => Constants.GAME_IDS.Contains(x)).WithMessage("Invalid game id.");

            RuleFor(x => x.CompanyId).NotNull().NotEmpty();
            RuleFor(x => x.CompanyId).MustAsync(BeAnExistingCompany).WithMessage("Company doesn't exist.").When(x => !x.CompanyId.IsNullOrBlank());
        }

        private async Task<bool> NotBeAnExistingUserName(CheckIdentityAvailabilityQuery query, CancellationToken arg2)
        {
            return !await _userRepository.BeAnExistingUserName(query.UserName, companyId: query.CompanyId);
        }

        private async Task<bool> NotBeAnExistingUserEmail(CheckIdentityAvailabilityQuery query, CancellationToken arg2)
        {
            return !await _userRepository.BeAnExistingUserEmail(query.Email, companyId: query.CompanyId);
        }

        private async Task<bool> BeAnExistingCompany(string userName, CancellationToken arg2)
        {
            return await _companyRepository.BeAnExistingCompany(userName);
        }
    }
}
