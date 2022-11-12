using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventGamesCore
{
    public class GetGamePrizesQueryHandler : IRequestHandler<GetGamePrizesQuery, QueryRecordsResponse<GamePrize>>
    {
        #region Fields

        private readonly ILogger<GetGamePrizesQueryHandler> _logger;
        private readonly GetGamePrizesQueryValidator _validator;
        private readonly IGamePrizeRepository _repository;

        #endregion

        #region Ctor

        public GetGamePrizesQueryHandler(ILogger<GetGamePrizesQueryHandler> logger, GetGamePrizesQueryValidator validator, IGamePrizeRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordsResponse<GamePrize>> Handle(GetGamePrizesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return await _repository.GetGamePrizes(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordsResponse<GamePrize>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }

        #endregion
    }
}
