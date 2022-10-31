using SpaceShooterCore.Extensions;
using FluentValidation;

namespace SpaceShooterCore
{
    public class GenerateSessionCommandValidator : AbstractValidator<GenerateSessionCommand>
    {
        private readonly IUserRepository _userRepository;

        public GenerateSessionCommandValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.UserId).NotNull().NotEmpty();
            RuleFor(x => x.UserId).MustAsync(BeAnExistingUser).WithMessage("Username or email doesn't exists.").When(x => !x.UserId.IsNullOrBlank());
        }

        private async Task<bool> BeAnExistingUser(string userId, CancellationToken arg2)
        {
            return await _userRepository.BeAnExistingUser(userId);
        }
    }
}
