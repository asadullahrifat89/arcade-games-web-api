using MediatR;
using Microsoft.Extensions.Logging;

namespace AstroOdysseyCore
{
    public class GetGameScoresQueryHandler : IRequestHandler<GetGameScoresQuery, QueryRecordsResponse<GameScore>>
    {
        #region Fields

        private readonly ILogger<GetGameScoresQueryHandler> _logger;
        private readonly GetGameScoresQueryValidator _validator;
        private readonly IGameScoreRepository _repository;

        #endregion

        #region Ctor

        public GetGameScoresQueryHandler(ILogger<GetGameScoresQueryHandler> logger, GetGameScoresQueryValidator validator, IGameScoreRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordsResponse<GameScore>> Handle(GetGameScoresQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return await _repository.GetGameScores(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordsResponse<GameScore>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }


        #endregion
    }
}
