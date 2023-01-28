using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventGamesCore
{
    public class GetGameScheduleQueryHandler : IRequestHandler<GetGameScheduleQuery, QueryRecordResponse<GameSchedule>>
    {
        #region Fields

        private readonly ILogger<GetGameScheduleQueryHandler> _logger;
        private readonly GetGameScheduleQueryValidator _validator;
        private readonly IGameScheduleRepository _repository;

        #endregion

        #region Ctor

        public GetGameScheduleQueryHandler(ILogger<GetGameScheduleQueryHandler> logger, GetGameScheduleQueryValidator validator, IGameScheduleRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<QueryRecordResponse<GameSchedule>> Handle(GetGameScheduleQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(query, cancellationToken);
                validationResult.EnsureValidResult();

                return await _repository.GetGameSchedule(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new QueryRecordResponse<GameSchedule>().BuildErrorResponse(new ErrorResponse().BuildExternalError(ex.Message));
            }
        }

        #endregion
    }
}
