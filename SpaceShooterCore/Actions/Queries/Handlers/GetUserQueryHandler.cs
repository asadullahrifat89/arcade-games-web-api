using MediatR;
using Microsoft.Extensions.Logging;

namespace SpaceShooterCore
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, QueryRecordResponse<User>>
    {
        #region Fields

        private readonly ILogger<GetUserQueryHandler> _logger;
        private readonly GetUserQueryValidator _validator;
        private readonly IUserRepository _repository;

        #endregion

        #region Ctor

        public GetUserQueryHandler(
            ILogger<GetUserQueryHandler> logger,
            GetUserQueryValidator validator,
            IUserRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordResponse<User>> Handle(GetUserQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return await _repository.GetUser(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordResponse<User>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }


        #endregion
    }
}
