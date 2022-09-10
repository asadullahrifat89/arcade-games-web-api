using MediatR;
using Microsoft.Extensions.Logging;

namespace AstroOdysseyCore
{
    public class SubmitGameScoreCommandHandler : IRequestHandler<SubmitGameScoreCommand, ServiceResponse>
    {
        #region Fields

        private readonly ILogger<SubmitGameScoreCommandHandler> _logger;
        private readonly SubmitGameScoreCommandValidator _validator;
        private readonly IGameScoreRepository _repository;

        #endregion

        #region Ctor

        public SubmitGameScoreCommandHandler(ILogger<SubmitGameScoreCommandHandler> logger, SubmitGameScoreCommandValidator validator, IGameScoreRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<ServiceResponse> Handle(SubmitGameScoreCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(command, cancellationToken);
                validationResult.EnsureValidResult();

                var response = await _repository.SubmitGameScore(command);

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
