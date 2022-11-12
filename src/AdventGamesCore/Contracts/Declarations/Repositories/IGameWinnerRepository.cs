namespace AdventGamesCore
{
    public interface IGameWinnerRepository
    {
        Task<QueryRecordsResponse<GameWinner>> GetGameWinners(GetGameWinnersQuery query);
    }
}
