using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventGamesCore
{
    public class CheckIdentityAvailabilityQueryHandler : IRequestHandler<CheckIdentityAvailabilityQuery, QueryRecordResponse<bool>>
    {
        #region Fields

        private readonly ILogger<CheckIdentityAvailabilityQueryHandler> _logger;
        private readonly CheckIdentityAvailabilityQueryValidator _validator;

        #endregion

        #region Ctor

        public CheckIdentityAvailabilityQueryHandler(
            ILogger<CheckIdentityAvailabilityQueryHandler> logger,
            CheckIdentityAvailabilityQueryValidator validator)
        {
            _logger = logger;
            _validator = validator;            
        }

        #endregion

        #region Methods

        public async Task<QueryRecordResponse<bool>> Handle(CheckIdentityAvailabilityQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return new QueryRecordResponse<bool>().BuildSuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordResponse<bool>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }


        #endregion
    }
}
