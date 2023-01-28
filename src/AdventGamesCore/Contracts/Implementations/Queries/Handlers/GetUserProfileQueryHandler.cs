using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventGamesCore
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, QueryRecordResponse<UserProfile>>
    {
        #region Fields

        private readonly ILogger<GetUserProfileQueryHandler> _logger;
        private readonly GetUserProfileQueryValidator _validator;
        private readonly IUserRepository _repository;

        #endregion

        #region Ctor

        public GetUserProfileQueryHandler(
            ILogger<GetUserProfileQueryHandler> logger,
            GetUserProfileQueryValidator validator,
            IUserRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordResponse<UserProfile>> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return await _repository.GetUserProfile(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordResponse<UserProfile>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }


        #endregion
    }
}
