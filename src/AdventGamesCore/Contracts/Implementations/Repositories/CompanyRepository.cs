using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AdventGamesCore
{
    public class CompanyRepository : ICompanyRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDBService;
        private readonly IOptions<CompanyOptions> _companysOptions;

        #endregion

        #region Ctor

        public CompanyRepository(IMongoDbService mongoDBService, IOptions<CompanyOptions> options)
        {
            _mongoDBService = mongoDBService;
            _companysOptions = options;
        }

        #endregion

        #region Methods

        #region Public

        public async Task<bool> BeAnExistingCompany(string id)
        {
            var filter = Builders<Company>.Filter.Eq(x => x.Id, id);
            return await _mongoDBService.Exists(filter);
        }

        public async Task<QueryRecordResponse<Company>> GetCompany(GetCompanyQuery query)
        {
            var filter = Builders<Company>.Filter.Eq(x => x.Id, query.CompanyId);

            var result = await _mongoDBService.FindOne(filter);

            return result is not null
             ? new QueryRecordResponse<Company>().BuildSuccessResponse(result)
             : new QueryRecordResponse<Company>().BuildErrorResponse(new ErrorResponse().BuildExternalError("Company not found."));

        }

        public async Task LoadJson()
        {
            await _mongoDBService.DropCollection<Company>();

            var companies = _companysOptions.Value.Companies;

            if (companies is not null && companies.Length > 0)
                await _mongoDBService.InsertDocuments(companies);
        }

        #endregion

        #endregion
    }
}
