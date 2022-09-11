using FluentValidation;

namespace AstroOdysseyCore
{
    public class AuthenticationCommandValidator : AbstractValidator<AuthenticateCommand>
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationCommandValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.UserName).NotNull().NotEmpty();
            RuleFor(x => x.UserName).MustAsync(BeAnExistingUserNameOrEmail).WithMessage("Username or email doesn't exists.");

            RuleFor(x => x.Password).NotNull().NotEmpty();
            RuleFor(x => x).MustAsync(BeValidUser).WithMessage("Invalid pasword.");
        }

        private async Task<bool> BeValidUser(AuthenticateCommand command, CancellationToken arg2)
        {
            return await _userRepository.BeValidUser(userNameOrEmail: command.UserName, password: command.Password);
        }

        private async Task<bool> BeAnExistingUserNameOrEmail(string userName, CancellationToken arg2)
        {
            return await _userRepository.BeAnExistingUserNameOrEmail(userName);
        }
    }
}
