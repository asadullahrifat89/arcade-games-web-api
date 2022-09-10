using MediatR;
using Microsoft.Extensions.Logging;

namespace AstroOdysseyCore
{
    public class SignupCommandHandler : IRequestHandler<SignupCommand, ServiceResponse>
    {
        #region Fields

        private readonly ILogger<SignupCommandHandler> _logger;
        private readonly SignupCommandValidator _validator;
        private readonly IUserRepository _repository;

        #endregion

        #region Ctor

        public SignupCommandHandler(ILogger<SignupCommandHandler> logger, SignupCommandValidator validator, IUserRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<ServiceResponse> Handle(SignupCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(command, cancellationToken);
                validationResult.EnsureValidResult();

                var response = await _repository.Signup(command);              

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
