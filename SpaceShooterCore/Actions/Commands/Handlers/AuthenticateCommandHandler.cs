using MediatR;
using Microsoft.Extensions.Logging;

namespace SpaceShooterCore
{
    public class AuthenticateCommandHandler : IRequestHandler<AuthenticateCommand, ServiceResponse>
    {
        #region Fields

        private readonly ILogger<AuthenticateCommandHandler> _logger;
        private readonly AuthenticateCommandValidator _validator;
        private readonly ISessionRepository _repository;

        #endregion

        #region Ctor

        public AuthenticateCommandHandler(
            ILogger<AuthenticateCommandHandler> logger,
            AuthenticateCommandValidator validator,
            ISessionRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<ServiceResponse> Handle(AuthenticateCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(command, cancellationToken);
                validationResult.EnsureValidResult();

                var response = await _repository.Authenticate(command);

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
