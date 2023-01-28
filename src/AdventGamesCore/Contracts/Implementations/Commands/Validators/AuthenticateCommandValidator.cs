using AdventGamesCore.Extensions;
using FluentValidation;

namespace AdventGamesCore
{
    public class AuthenticateCommandValidator : AbstractValidator<AuthenticateCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;

        public AuthenticateCommandValidator(
            IUserRepository userRepository, 
            ICompanyRepository companyRepository)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;

            RuleFor(x => x.CompanyId).NotNull().NotEmpty();
            RuleFor(x => x.CompanyId).MustAsync(BeAnExistingCompany).WithMessage("Company doesn't exist.").When(x => !x.CompanyId.IsNullOrBlank());

            RuleFor(x => x.UserName).NotNull().NotEmpty();
            RuleFor(x => x).MustAsync(BeAnExistingUserNameOrEmail).WithMessage("Username or email doesn't exists.").When(x => !x.UserName.IsNullOrBlank());

            RuleFor(x => x.Password).NotNull().NotEmpty();
            RuleFor(x => x).MustAsync(BeValidUser).WithMessage("Invalid pasword.").When(x => !x.Password.IsNullOrBlank());
            
        }

        private async Task<bool> BeValidUser(AuthenticateCommand command, CancellationToken arg2)
        {
            return await _userRepository.BeValidUser(userNameOrEmail: command.UserName, password: command.Password);
        }

        private async Task<bool> BeAnExistingUserNameOrEmail(AuthenticateCommand command, CancellationToken arg2)
        {
            return await _userRepository.BeAnExistingUserNameOrEmail(userNameOrEmail: command.UserName, companyId: command.CompanyId);
        }

        private async Task<bool> BeAnExistingCompany(string userName, CancellationToken arg2)
        {
            return await _companyRepository.BeAnExistingCompany(userName);
        }
    }
}
