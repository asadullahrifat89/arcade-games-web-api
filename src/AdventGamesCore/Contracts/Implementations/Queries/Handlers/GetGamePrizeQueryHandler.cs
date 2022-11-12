using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventGamesCore
{
    public class GetGamePrizeQueryHandler : IRequestHandler<GetGamePrizeQuery, QueryRecordResponse<GamePrize>>
    {
        #region Fields

        private readonly ILogger<GetGamePrizeQueryHandler> _logger;
        private readonly GetGamePrizeQueryValidator _validator;
        private readonly IGamePrizeRepository _repository;

        #endregion

        #region Ctor

        public GetGamePrizeQueryHandler(ILogger<GetGamePrizeQueryHandler> logger, GetGamePrizeQueryValidator validator, IGamePrizeRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordResponse<GamePrize>> Handle(GetGamePrizeQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return await _repository.GetGamePrize(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordResponse<GamePrize>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }

        #endregion
    }
}
