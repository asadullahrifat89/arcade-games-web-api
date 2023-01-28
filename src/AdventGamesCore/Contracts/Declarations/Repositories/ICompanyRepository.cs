namespace AdventGamesCore
{
    public interface ICompanyRepository
    {
        Task<bool> BeAnExistingCompany(string id);

        Task<QueryRecordResponse<Company>> GetCompany(GetCompanyQuery query);

        //TODO: this is temporary, will be replaced by a proper set of APIs
        Task LoadJson();
    }
}
