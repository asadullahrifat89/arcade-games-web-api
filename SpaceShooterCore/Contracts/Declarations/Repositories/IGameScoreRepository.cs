namespace SpaceShooterCore
{
    public interface IGameScoreRepository
    {
        Task<ServiceResponse> SubmitGameScore(SubmitGameScoreCommand command);

        Task<QueryRecordsResponse<GameScore>> GetGameScores(GetGameScoresQuery query);
    }
}
