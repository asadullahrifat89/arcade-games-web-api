using AdventGamesCore.Extensions;
using FluentValidation;

namespace AdventGamesCore
{
    public class SubmitGameScoreCommandValidator : AbstractValidator<SubmitGameScoreCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ISessionRepository _sessionRepository;

        public SubmitGameScoreCommandValidator(
            IUserRepository userRepository,
            ICompanyRepository companyRepository,
            ISessionRepository sessionRepository)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;
            _sessionRepository = sessionRepository;

            RuleFor(x => x.SessionId).NotNull().NotEmpty();
            RuleFor(x => x).MustAsync(BeAnExistingSession).WithMessage("Session doesn't exists.").When(x => !x.SessionId.IsNullOrBlank());
            RuleFor(x => x).MustAsync(BeAnIncompleteSession).WithMessage("Session is no longer usable.").When(x => !x.SessionId.IsNullOrBlank());

            RuleFor(x => x.User).NotNull();

            RuleFor(x => x.User.UserId).NotNull().NotEmpty();
            RuleFor(x => x.User.UserId).MustAsync(BeAnExistingUser).WithMessage("User id doesnt exist.");

            RuleFor(x => x.User.UserName).NotNull().NotEmpty();
            RuleFor(x => x).MustAsync(BeAnExistingUserName).WithMessage("Username doesnt exist.");

            RuleFor(x => x.GameId).NotNull().NotEmpty();
            RuleFor(x => x.GameId).Must(x => Constants.GAME_IDS.Contains(x)).WithMessage("Invalid game id.").When(x => !x.GameId.IsNullOrBlank());

            RuleFor(x => x.CompanyId).NotNull().NotEmpty();
            RuleFor(x => x.CompanyId).MustAsync(BeAnExistingCompany).WithMessage("Company doesn't exist.").When(x => !x.CompanyId.IsNullOrBlank());

            RuleFor(x => x.Score).GreaterThan(0);
            RuleFor(x => x.Score).Must(x => x < double.MaxValue && x < 99999).WithMessage("Invalid game score").When(x => x.Score > 0);
        }

        private async Task<bool> BeAnExistingUser(string id, CancellationToken arg2)
        {
            return await _userRepository.BeAnExistingUser(id);
        }

        private async Task<bool> BeAnExistingUserName(SubmitGameScoreCommand command, CancellationToken arg2)
        {
            return await _userRepository.BeAnExistingUserName(userName: command.User.UserName, companyId: command.CompanyId);
        }

        private async Task<bool> BeAnExistingCompany(string userName, CancellationToken arg2)
        {
            return await _companyRepository.BeAnExistingCompany(userName);
        }

        private async Task<bool> BeAnExistingSession(SubmitGameScoreCommand command, CancellationToken arg2)
        {
            return await _sessionRepository.BeAnExistingSession(sessionId: command.SessionId, gameId: command.GameId);
        }

        private async Task<bool> BeAnIncompleteSession(SubmitGameScoreCommand command, CancellationToken arg2)
        {
            return await _sessionRepository.BeAnIncompleteSession(sessionId: command.SessionId, gameId: command.GameId);
        }
    }
}
