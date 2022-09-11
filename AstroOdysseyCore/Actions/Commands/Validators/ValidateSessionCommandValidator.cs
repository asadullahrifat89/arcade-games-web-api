using AstroOdysseyCore.Extensions;
using FluentValidation;

namespace AstroOdysseyCore
{
    public class ValidateSessionCommandValidator : AbstractValidator<ValidateSessionCommand>
    {
        private readonly ISessionRepository _sessionRepository;

        public ValidateSessionCommandValidator(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;

            RuleFor(x => x.SessionId).NotNull().NotEmpty();
            RuleFor(x => x.SessionId).MustAsync(BeAnExistingSession).WithMessage("Username or email doesn't exists.").When(x => !x.SessionId.IsNullOrBlank());

            RuleFor(x => x.GameId).NotNull().NotEmpty();
        }

        private async Task<bool> BeAnExistingSession(string sessionId, CancellationToken arg2)
        {
            return await _sessionRepository.BeAnExistingSession(sessionId);
        }
    }
}
