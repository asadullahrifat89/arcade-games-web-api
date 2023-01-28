using AdventGamesCore.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AdventGamesCore
{
    public class GamePrizeRepository : IGamePrizeRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDBService;
        private readonly IOptions<GamePrizeOptions> _gamePrizesOptions;

        #endregion

        #region Ctor

        public GamePrizeRepository(IMongoDbService mongoDBService, IOptions<GamePrizeOptions> options)
        {
            _mongoDBService = mongoDBService;
            _gamePrizesOptions = options;
        }

        #endregion

        #region Methods

        #region Public

        public async Task<QueryRecordsResponse<GamePrize>> GetGamePrizes(GetGamePrizesQuery query)
        {
            var filter = Builders<GamePrize>.Filter.And(Builders<GamePrize>.Filter.Eq(x => x.GameId, query.GameId), Builders<GamePrize>.Filter.Eq(x => x.CompanyId, query.CompanyId));

            if (query.SearchTerm is not null && !query.SearchTerm.IsNullOrBlank())
                filter &= Builders<GamePrize>.Filter.ElemMatch(x => x.PrizeDescriptions, c => c.Value.ToLowerInvariant().Contains(query.SearchTerm.ToLowerInvariant()));

            if (query.Day > 0)
                filter &= Builders<GamePrize>.Filter.AnyIn(x => x.Days, new[] { query.Day.Value });

            if (query.Culture is not null && !query.Culture.IsNullOrBlank())
                filter &= Builders<GamePrize>.Filter.ElemMatch(x => x.PrizeDescriptions, c => c.Culture == query.Culture);

            var count = await _mongoDBService.CountDocuments(filter);

            var results = await _mongoDBService.GetDocuments(
                filter: filter,
                skip: query.PageIndex * query.PageSize,
                limit: query.PageSize,
                sortOrder: SortOrder.Ascending,
                sortFieldName: nameof(GamePrize.Name));

            return new QueryRecordsResponse<GamePrize>().BuildSuccessResponse(
               count: results is not null ? count : 0,
               records: results is not null ? results.ToArray() : Array.Empty<GamePrize>());
        }

        public async Task<QueryRecordResponse<GamePrizeOfTheDay>> GetGamePrizeOfTheDay(GetGamePrizeOfTheDayQuery query)
        {
            GamePrize? result = await GetGamePrize(
                gameId: query.GameId,
                day: query.Day,
                companyId: query.CompanyId,
                culture: query.Culture);

            return result is not null
               ? new QueryRecordResponse<GamePrizeOfTheDay>().BuildSuccessResponse(GamePrizeOfTheDay.Initialize(result))
               : new QueryRecordResponse<GamePrizeOfTheDay>().BuildErrorResponse(new ErrorResponse().BuildExternalError("Game prize not found."));
        }

        public async Task<GamePlayResult> GetGamePlayResult(GameScore gameScore)
        {
            GamePlayResult gamePlayResult = new();

            _ = int.TryParse(gameScore.ScoreDay.Split('-')[0], out int day); // take the day part

            if (await GetGamePrize(gameId: gameScore.GameId, day: day, companyId: gameScore.CompanyId) is GamePrize gamePrize)
            {
                switch (gamePrize.WinningCriteria.CriteriaType)
                {
                    case WinningCriteriaType.DailyHighScore: // means no winning is decided now, the daily highest scorer will win at the end of the day
                        {
                            gamePlayResult = GamePlayResult.Initialize(
                                gamePrize: gamePrize,
                                winningDescriptions: gamePrize.WinningCriteria.CriteriaDescriptions);
                        }
                        break;
                    case WinningCriteriaType.ScoreThreshold: // if the game score meets score threadhold then the player wins now
                        {
                            if (gameScore.Score >= gamePrize.WinningCriteria.ScoreThreshold)
                            {
                                gamePlayResult = GamePlayResult.Initialize(
                                   gamePrize: gamePrize,
                                   winningDescriptions: gamePrize.WinningCriteria.WinningDescriptions);
                            }
                            else
                            {
                                // if the player didn't win then no need to show the prize url
                                gamePrize.PrizeUrls = Array.Empty<CultureValue>();

                                gamePlayResult = GamePlayResult.Initialize(
                                   gamePrize: gamePrize,
                                   winningDescriptions: gamePrize.WinningCriteria.MotivationDescriptions is not null && gamePrize.WinningCriteria.MotivationDescriptions.Any()
                                   ? gamePrize.WinningCriteria.MotivationDescriptions
                                   : gamePrize.WinningCriteria.CriteriaDescriptions);
                            }
                        }
                        break;
                }
            }

            return gamePlayResult;
        }

        public async Task LoadJson()
        {
            await _mongoDBService.DropCollection<GamePrize>();

            var prizes = _gamePrizesOptions.Value.GamePrizes;

            if (prizes is not null && prizes.Length > 0)
                await _mongoDBService.InsertDocuments(prizes);
        }

        #endregion

        #region Private

        private async Task<GamePrize> GetGamePrize(string gameId, int day, string companyId, string culture = "")
        {
            var filter = Builders<GamePrize>.Filter.And(Builders<GamePrize>.Filter.Eq(x => x.GameId, gameId),
            Builders<GamePrize>.Filter.Eq(x => x.CompanyId, companyId),
            Builders<GamePrize>.Filter.AnyIn(x => x.Days, new[] { day }));

            if (!culture.IsNullOrBlank())
                filter &= Builders<GamePrize>.Filter.ElemMatch(x => x.PrizeDescriptions, c => c.Culture == culture);

            var result = await _mongoDBService.FindOne(filter);

            return result;
        }

        #endregion

        #endregion
    }
}
