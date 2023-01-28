namespace AdventGamesCore
{
    public interface IGameScheduleRepository
    {
        Task<QueryRecordResponse<GameSchedule>> GetGameSchedule(GetGameScheduleQuery query);

        //TODO: this is temporary, will be replaced by a proper set of APIs
        Task LoadJson();
    }
}
