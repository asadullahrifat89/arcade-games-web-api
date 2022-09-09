namespace AstroOdysseyCore
{
    public interface IGameScoreRepository
    {
        Task<ActionCommandResponse> SubmitGameScore(SubmitGameScoreCommand command);
    }
}
