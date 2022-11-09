using FluentValidation;

namespace AdventGamesCore
{
    public class SubmitGameScoreCommandValidator : AbstractValidator<SubmitGameScoreCommand>
    {
        private readonly IUserRepository _userRepository;

        public SubmitGameScoreCommandValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.User).NotNull();

            RuleFor(x => x.User.UserId).NotNull().NotEmpty();
            RuleFor(x => x.User.UserId).MustAsync(BeAnExistingUser).WithMessage("User id doesnt exist.");

            RuleFor(x => x.User.UserName).NotNull().NotEmpty();
            RuleFor(x => x.User.UserName).MustAsync(BeAnExistingUserName).WithMessage("Username doesnt exist.");

            RuleFor(x => x.User.UserEmail).NotNull().NotEmpty();
            RuleFor(x => x.User.UserEmail).MustAsync(BeAnExistingUserEmail).WithMessage("User email doesnt exist.");
                        
            RuleFor(x => x.GameId).NotNull().NotEmpty();
        }

        private async Task<bool> BeAnExistingUser(string id, CancellationToken arg2)
        {
            return await _userRepository.BeAnExistingUser(id);
        }

        private async Task<bool> BeAnExistingUserName(string userName, CancellationToken arg2)
        {
            return await _userRepository.BeAnExistingUserName(userName);
        }

        private async Task<bool> BeAnExistingUserEmail(string userEmail, CancellationToken arg2)
        {
            return await _userRepository.BeAnExistingUserEmail(userEmail);
        }
    }
}
