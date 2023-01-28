using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventGamesCore
{
    public class GetGameHighScoresQueryHandler : IRequestHandler<GetGameHighScoresQuery, QueryRecordsResponse<GameHighScore>>
    {
        #region Fields

        private readonly ILogger<GetGameHighScoresQueryHandler> _logger;
        private readonly GetGameHighScoresQueryValidator _validator;
        private readonly IGameScoreRepository _repository;

        #endregion

        #region Ctor

        public GetGameHighScoresQueryHandler(ILogger<GetGameHighScoresQueryHandler> logger, GetGameHighScoresQueryValidator validator, IGameScoreRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordsResponse<GameHighScore>> Handle(GetGameHighScoresQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return await _repository.GetGameHighScores(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordsResponse<GameHighScore>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }


        #endregion
    }
}
