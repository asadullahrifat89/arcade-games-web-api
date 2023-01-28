using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AdventGamesCore
{
    public class SeasonRepository : ISeasonRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDBService;
        private readonly IOptions<SeasonOptions> _gameSeasonsOptions;

        #endregion

        #region Ctor

        public SeasonRepository(IMongoDbService mongoDBService, IOptions<SeasonOptions> options)
        {
            _mongoDBService = mongoDBService;
            _gameSeasonsOptions = options;
        }

        #endregion

        #region Methods

        #region Public

        public async Task<QueryRecordResponse<Season>> GetSeason(GetSeasonQuery query)
        {
            var now = DateTime.UtcNow;

            var filter = Builders<Season>.Filter.Eq(x => x.CompanyId, query.CompanyId);
            filter &= Builders<Season>.Filter.Where(x => x.StartTime <= now && x.EndTime >= now);

            var result = await _mongoDBService.FindOne(filter);

            return result is not null
             ? new QueryRecordResponse<Season>().BuildSuccessResponse(result)
             : new QueryRecordResponse<Season>().BuildErrorResponse(new ErrorResponse().BuildExternalError("No season is ongoing right now."));

        }

        public async Task LoadJson()
        {
            await _mongoDBService.DropCollection<Season>();

            var seasons = _gameSeasonsOptions.Value.Seasons;

            if (seasons is not null && seasons.Length > 0)
                await _mongoDBService.InsertDocuments(seasons);
        }

        #endregion

        #endregion
    }
}
