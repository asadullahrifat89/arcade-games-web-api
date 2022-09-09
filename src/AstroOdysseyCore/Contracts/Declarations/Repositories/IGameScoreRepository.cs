namespace AstroOdysseyCore
{
    public interface IGameScoreRepository
    {
        Task<ActionCommandResponse> SubmitGameScore(SubmitGameScoreCommand command);

        Task<QueryRecordsResponse<GameScore>> GetGameScores(GetGameScoresQuery query);
    }
}
