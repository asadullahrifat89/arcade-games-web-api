using AdventGamesCore.Extensions;
using FluentValidation;

namespace AdventGamesCore
{
    public class SignupCommandValidator : AbstractValidator<SignupCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;

        public SignupCommandValidator(
            IUserRepository userRepository,
            ICompanyRepository companyRepository)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;

            RuleFor(x => x.FullName).NotNull().NotEmpty();

            RuleFor(x => x.UserName).NotNull().NotEmpty();
            RuleFor(x => x).MustAsync(NotBeAnExistingUserName).WithMessage("Username already exists.").When(x => !x.UserName.IsNullOrBlank());

            RuleFor(x => x.Email).NotNull().NotEmpty();
            RuleFor(x => x).MustAsync(NotBeAnExistingUserEmail).WithMessage("Email already exists.").When(x => !x.Email.IsNullOrBlank());

            RuleFor(x => x.Password).NotNull().NotEmpty();

            RuleFor(x => x.GameId).NotNull().NotEmpty();
            RuleFor(x => x.GameId).Must(x => Constants.GAME_IDS.Contains(x)).WithMessage("Invalid game id.").When(x => !x.GameId.IsNullOrBlank());

            RuleFor(x => x.CompanyId).NotNull().NotEmpty();
            RuleFor(x => x.CompanyId).MustAsync(BeAnExistingCompany).WithMessage("Company doesn't exist.").When(x => !x.CompanyId.IsNullOrBlank());
        }

        private async Task<bool> NotBeAnExistingUserName(SignupCommand command, CancellationToken arg2)
        {
            return !await _userRepository.BeAnExistingUserName(userName: command.UserName, companyId: command.CompanyId);
        }

        private async Task<bool> NotBeAnExistingUserEmail(SignupCommand command, CancellationToken arg2)
        {
            return !await _userRepository.BeAnExistingUserEmail(userEmail: command.Email, companyId: command.CompanyId);
        }

        private async Task<bool> BeAnExistingCompany(string userName, CancellationToken arg2)
        {
            return await _companyRepository.BeAnExistingCompany(userName);
        }
    }
}
