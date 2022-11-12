using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventGamesCore
{
    public class GenerateSessionCommandHandler : IRequestHandler<GenerateSessionCommand, ServiceResponse>
    {
        #region Fields

        private readonly ILogger<GenerateSessionCommandHandler> _logger;
        private readonly GenerateSessionCommandValidator _validator;
        private readonly ISessionRepository _repository;

        #endregion

        #region Ctor

        public GenerateSessionCommandHandler(
            ILogger<GenerateSessionCommandHandler> logger,
            GenerateSessionCommandValidator validator,
            ISessionRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<ServiceResponse> Handle(GenerateSessionCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(command, cancellationToken);
                validationResult.EnsureValidResult();

                var response = await _repository.GenerateSession(command);

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
