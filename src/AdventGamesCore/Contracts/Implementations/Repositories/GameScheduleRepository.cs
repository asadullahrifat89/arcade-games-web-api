using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AdventGamesCore
{
    public class GameScheduleRepository : IGameScheduleRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDBService;
        private readonly IOptions<GameScheduleOptions> _gameGameSchedulesOptions;

        #endregion

        #region Ctor

        public GameScheduleRepository(IMongoDbService mongoDBService, IOptions<GameScheduleOptions> options)
        {
            _mongoDBService = mongoDBService;
            _gameGameSchedulesOptions = options;
        }

        #endregion

        #region Methods

        #region Public

        public async Task<QueryRecordResponse<GameSchedule>> GetGameSchedule(GetGameScheduleQuery query)
        {
            var now = DateTime.UtcNow;

            var filter = Builders<GameSchedule>.Filter.Eq(x => x.CompanyId, query.CompanyId);
            filter &= Builders<GameSchedule>.Filter.Eq(x => x.SeasonId, query.SeasonId);
            filter &= Builders<GameSchedule>.Filter.Where(x => now >= x.StartTime && now <= x.EndTime);

            var result = await _mongoDBService.FindOne(filter);

            return result is not null
             ? new QueryRecordResponse<GameSchedule>().BuildSuccessResponse(result)
             : new QueryRecordResponse<GameSchedule>().BuildErrorResponse(new ErrorResponse().BuildExternalError("Game schedule not found."));
        }

        public async Task LoadJson()
        {
            await _mongoDBService.DropCollection<GameSchedule>();

            var gameSchedules = _gameGameSchedulesOptions.Value.GameSchedules;

            if (gameSchedules is not null && gameSchedules.Length > 0)
                await _mongoDBService.InsertDocuments(gameSchedules);
        }

        #endregion

        #endregion
    }
}
