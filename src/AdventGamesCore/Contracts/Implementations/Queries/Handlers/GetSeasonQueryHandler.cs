using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventGamesCore
{
    public class GetSeasonQueryHandler : IRequestHandler<GetSeasonQuery, QueryRecordResponse<Season>>
    {
        #region Fields

        private readonly ILogger<GetSeasonQueryHandler> _logger;
        private readonly GetSeasonQueryValidator _validator;
        private readonly ISeasonRepository _repository;

        #endregion

        #region Ctor

        public GetSeasonQueryHandler(ILogger<GetSeasonQueryHandler> logger, GetSeasonQueryValidator validator, ISeasonRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordResponse<Season>> Handle(GetSeasonQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return await _repository.GetSeason(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordResponse<Season>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }

        #endregion
    }
}
