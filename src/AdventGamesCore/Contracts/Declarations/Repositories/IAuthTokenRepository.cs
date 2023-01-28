namespace AdventGamesCore
{
    public interface IAuthTokenRepository
    {
        Task<ServiceResponse> Authenticate(AuthenticateCommand command);

        Task<bool> BeAnExistingRefreshToken(string refreshToken, string companyId);

        Task<ServiceResponse> ValidateToken(ValidateTokenCommand command);
    }
}
