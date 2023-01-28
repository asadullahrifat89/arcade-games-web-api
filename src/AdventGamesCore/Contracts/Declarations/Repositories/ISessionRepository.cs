namespace AdventGamesCore
{
    public interface ISessionRepository
    {
        Task<bool> BeAnExistingSession(string sessionId, string gameId);

        Task<bool> BeAnIncompleteSession(string sessionId, string gameId);

        Task<ServiceResponse> GenerateSession(GenerateSessionCommand command);

        Task<bool> CompleteSession(string sessionId, string gameId);
    }
}
