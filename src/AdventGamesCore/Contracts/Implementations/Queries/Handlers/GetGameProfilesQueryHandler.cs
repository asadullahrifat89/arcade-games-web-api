using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventGamesCore
{
    public class GetGameProfilesQueryHandler : IRequestHandler<GetGameProfilesQuery, QueryRecordsResponse<GameProfile>>
    {
        #region Fields

        private readonly ILogger<GetGameProfilesQueryHandler> _logger;
        private readonly GetGameProfilesQueryValidator _validator;
        private readonly IGameProfileRepository _repository;

        #endregion

        #region Ctor

        public GetGameProfilesQueryHandler(ILogger<GetGameProfilesQueryHandler> logger, GetGameProfilesQueryValidator validator, IGameProfileRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordsResponse<GameProfile>> Handle(GetGameProfilesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return await _repository.GetGameProfiles(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordsResponse<GameProfile>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }


        #endregion
    }
}
