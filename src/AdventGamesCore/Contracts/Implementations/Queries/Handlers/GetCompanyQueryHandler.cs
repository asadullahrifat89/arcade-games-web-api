using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventGamesCore
{
    public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, QueryRecordResponse<Company>>
    {
        #region Fields

        private readonly ILogger<GetCompanyQueryHandler> _logger;
        private readonly GetCompanyQueryValidator _validator;
        private readonly ICompanyRepository _repository;

        #endregion

        #region Ctor

        public GetCompanyQueryHandler(ILogger<GetCompanyQueryHandler> logger, GetCompanyQueryValidator validator, ICompanyRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordResponse<Company>> Handle(GetCompanyQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return await _repository.GetCompany(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordResponse<Company>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }

        #endregion
    }
}
