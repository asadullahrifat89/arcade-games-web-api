using FluentValidation;

namespace AdventGamesCore
{
    public class CheckIdentityAvailabilityQueryValidator : AbstractValidator<CheckIdentityAvailabilityQuery>
    {
        private readonly IUserRepository _userRepository;

        public CheckIdentityAvailabilityQueryValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.UserName).NotNull().NotEmpty();
            RuleFor(x => x.UserName).MustAsync(NotBeAnExistingUserName).WithMessage("Username already exists.");

            RuleFor(x => x.Email).NotNull().NotEmpty();
            RuleFor(x => x.Email).MustAsync(NotBeAnExistingUserEmail).WithMessage("Email already exists.");            

            RuleFor(x => x.GameId).NotNull().NotEmpty();
            RuleFor(x => x.GameId).Must(x => Constants.GAME_IDS.Contains(x)).WithMessage("Invalid game id.");
        }

        private async Task<bool> NotBeAnExistingUserName(string userName, CancellationToken arg2)
        {
            return !await _userRepository.BeAnExistingUserName(userName);
        }

        private async Task<bool> NotBeAnExistingUserEmail(string userEmail, CancellationToken arg2)
        {
            return !await _userRepository.BeAnExistingUserEmail(userEmail);
        }
    }
}
