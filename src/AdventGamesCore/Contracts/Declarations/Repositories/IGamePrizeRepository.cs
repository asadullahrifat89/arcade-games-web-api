namespace AdventGamesCore
{
    public interface IGamePrizeRepository
    {
        Task<QueryRecordsResponse<GamePrize>> GetGamePrizes(GetGamePrizesQuery query);

        Task<QueryRecordResponse<GamePrize>> GetGamePrize(GetGamePrizeQuery query);

        Task<GamePlayResult> GetGamePlayResult(GameScore gameScore);

        //TODO: this is temporary, will be replaced by a proper set of APIs
        Task LoadGamePrizesFromJson();
    }
}
