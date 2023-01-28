using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventGamesCore
{
    public class GetUserProfilesQueryHandler : IRequestHandler<GetUserProfilesQuery, QueryRecordsResponse<UserProfile>>
    {
        #region Fields

        private readonly ILogger<GetUserProfilesQueryHandler> _logger;
        private readonly GetUserProfilesQueryValidator _validator;
        private readonly IUserRepository _repository;

        #endregion

        #region Ctor

        public GetUserProfilesQueryHandler(ILogger<GetUserProfilesQueryHandler> logger, GetUserProfilesQueryValidator validator, IUserRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordsResponse<UserProfile>> Handle(GetUserProfilesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return await _repository.GetUserProfiles(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordsResponse<UserProfile>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }


        #endregion
    }
}
