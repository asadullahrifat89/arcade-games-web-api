using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventGamesCore
{
    public class GetGameWinnersQueryHandler : IRequestHandler<GetGameWinnersQuery, QueryRecordsResponse<GameWinner>>
    {
        #region Fields

        private readonly ILogger<GetGameWinnersQueryHandler> _logger;
        private readonly GetGameWinnersQueryValidator _validator;
        private readonly IGameWinnerRepository _repository;

        #endregion

        #region Ctor

        public GetGameWinnersQueryHandler(ILogger<GetGameWinnersQueryHandler> logger, GetGameWinnersQueryValidator validator, IGameWinnerRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordsResponse<GameWinner>> Handle(GetGameWinnersQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return await _repository.GetGameWinners(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordsResponse<GameWinner>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }


        #endregion
    }
}
