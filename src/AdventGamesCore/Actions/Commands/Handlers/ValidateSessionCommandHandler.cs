using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventGamesCore
{
    public class ValidateSessionCommandHandler : IRequestHandler<ValidateSessionCommand, ServiceResponse>
    {
        #region Fields

        private readonly ILogger<ValidateSessionCommandHandler> _logger;
        private readonly ValidateSessionCommandValidator _validator;
        private readonly ISessionRepository _repository;

        #endregion

        #region Ctor

        public ValidateSessionCommandHandler(
            ILogger<ValidateSessionCommandHandler> logger,
            ValidateSessionCommandValidator validator,
            ISessionRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<ServiceResponse> Handle(ValidateSessionCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(command, cancellationToken);
                validationResult.EnsureValidResult();

                var response = await _repository.ValidateSession(command);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Response.Build().BuildErrorResponse(ex.Message);
            }
        }

        #endregion
    }
}
