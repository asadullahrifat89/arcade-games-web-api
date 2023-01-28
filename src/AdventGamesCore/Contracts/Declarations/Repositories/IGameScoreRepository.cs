namespace AdventGamesCore
{
    public interface IGameScoreRepository
    {
        Task<ServiceResponse> SubmitGameScore(SubmitGameScoreCommand command);

        Task<QueryRecordsResponse<GameScore>> GetGameScores(GetGameScoresQuery query);

        Task<QueryRecordsResponse<GameHighScore>> GetGameHighScores(GetGameHighScoresQuery query);

        Task BanHackers();
    }
}
