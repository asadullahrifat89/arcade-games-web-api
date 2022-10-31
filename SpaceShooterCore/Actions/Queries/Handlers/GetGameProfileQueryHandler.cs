using MediatR;
using Microsoft.Extensions.Logging;

namespace SpaceShooterCore
{
    public class GetGameProfileQueryHandler : IRequestHandler<GetGameProfileQuery, QueryRecordResponse<GameProfile>>
    {
        #region Fields

        private readonly ILogger<GetGameProfileQueryHandler> _logger;
        private readonly GetGameProfileQueryValidator _validator;
        private readonly IGameProfileRepository _repository;

        #endregion

        #region Ctor

        public GetGameProfileQueryHandler(ILogger<GetGameProfileQueryHandler> logger, GetGameProfileQueryValidator validator, IGameProfileRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordResponse<GameProfile>> Handle(GetGameProfileQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return await _repository.GetGameProfile(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordResponse<GameProfile>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }


        #endregion
    }
}
