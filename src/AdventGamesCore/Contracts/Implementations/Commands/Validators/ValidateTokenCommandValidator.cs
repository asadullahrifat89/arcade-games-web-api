using AdventGamesCore.Extensions;
using FluentValidation;

namespace AdventGamesCore
{
    public class ValidateTokenCommandValidator : AbstractValidator<ValidateTokenCommand>
    {
        private readonly IAuthTokenRepository _authTokenRepository;
        private readonly ICompanyRepository _companyRepository;

        public ValidateTokenCommandValidator(
            IAuthTokenRepository authTokenRepository,
            ICompanyRepository companyRepository)
        {
            _authTokenRepository = authTokenRepository;
            _companyRepository = companyRepository;

            RuleFor(x => x.CompanyId).NotNull().NotEmpty();
            RuleFor(x => x.CompanyId).MustAsync(BeAnExistingCompany).WithMessage("Company doesn't exist.").When(x => !x.CompanyId.IsNullOrBlank());

            RuleFor(x => x.RefreshToken).NotNull().NotEmpty();
            RuleFor(x => x).MustAsync(BeAnExistingRefreshToken).WithMessage("Refresh token doesn't exists.").When(x => !x.RefreshToken.IsNullOrBlank());
        }

        private async Task<bool> BeAnExistingRefreshToken(ValidateTokenCommand command, CancellationToken arg2)
        {
            return await _authTokenRepository.BeAnExistingRefreshToken(refreshToken: command.RefreshToken, companyId: command.CompanyId);
        }

        private async Task<bool> BeAnExistingCompany(string userName, CancellationToken arg2)
        {
            return await _companyRepository.BeAnExistingCompany(userName);
        }
    }
}
