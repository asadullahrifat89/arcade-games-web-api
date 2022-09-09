using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AstroOdysseyCore
{
    public class SignupCommandHandler : IRequestHandler<SignupCommand, QueryRecordResponse<GameProfile>>
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

        public async Task<QueryRecordResponse<GameProfile>> Handle(SignupCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(command, cancellationToken);
                validationResult.EnsureValidResult();

                var result = await _repository.Signup(command);

                return new QueryRecordResponse<GameProfile>().BuildSuccessResponse(result);
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
