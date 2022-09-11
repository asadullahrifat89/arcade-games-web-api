namespace AstroOdysseyCore
{
    public interface IAuthenticationRepository
    {
        Task<bool> BeAnExistingSession(string sessionId);

        Task<ServiceResponse> Authenticate(AuthenticateCommand command);

        Task<ServiceResponse> GenerateSession(GenerateSessionCommand command);

        Task<ServiceResponse> ValidateSession(ValidateSessionCommand command);
    }
}
