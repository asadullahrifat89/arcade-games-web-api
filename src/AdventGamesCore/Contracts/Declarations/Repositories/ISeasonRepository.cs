namespace AdventGamesCore
{
    public interface ISeasonRepository 
    {
        Task<QueryRecordResponse<Season>> GetSeason(GetSeasonQuery query);

        //TODO: this is temporary, will be replaced by a proper set of APIs
        Task LoadJson();
    }
}
