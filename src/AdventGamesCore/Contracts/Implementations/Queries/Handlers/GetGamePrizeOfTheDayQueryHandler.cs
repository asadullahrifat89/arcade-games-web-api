using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventGamesCore
{
    public class GetGamePrizeOfTheDayQueryHandler : IRequestHandler<GetGamePrizeOfTheDayQuery, QueryRecordResponse<GamePrizeOfTheDay>>
    {
        #region Fields

        private readonly ILogger<GetGamePrizeOfTheDayQueryHandler> _logger;
        private readonly GetGamePrizeOfTheDayQueryValidator _validator;
        private readonly IGamePrizeRepository _repository;

        #endregion

        #region Ctor

        public GetGamePrizeOfTheDayQueryHandler(ILogger<GetGamePrizeOfTheDayQueryHandler> logger, GetGamePrizeOfTheDayQueryValidator validator, IGamePrizeRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordResponse<GamePrizeOfTheDay>> Handle(GetGamePrizeOfTheDayQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return await _repository.GetGamePrizeOfTheDay(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordResponse<GamePrizeOfTheDay>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }

        #endregion
    }
}
