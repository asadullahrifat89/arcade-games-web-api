using FluentValidation;

namespace AdventGamesCore
{
    public class GetGameHighScoresQueryValidator : AbstractValidator<GetGameHighScoresQuery>
    {
        public GetGameHighScoresQueryValidator()
        {
            RuleFor(x => x.GameId).NotNull().NotEmpty();
            RuleFor(x => x.GameId).Must(x => Constants.GAME_IDS.Contains(x)).WithMessage("Invalid game id.");

            RuleFor(x => x.Filter).Must(x => Enum.GetValues<HighScoreFilter>().Contains(x)).WithMessage("Invalid filter.");

            RuleFor(x => x.FromDate).NotNull().When(x => x.Filter == HighScoreFilter.DATE || x.Filter == HighScoreFilter.DATERANGE).WithMessage("From date cannot be null.");
            RuleFor(x => x.ToDate).NotNull().When(x => x.Filter == HighScoreFilter.DATERANGE).WithMessage("To date cannot be null.");

            RuleFor(x => x).Must(x => x.FromDate <= x.ToDate).WithMessage("Invalid date range.").When(x => x.FromDate != DateTime.MinValue && x.ToDate != DateTime.MinValue);

            RuleFor(x => x.Limit).GreaterThan(0);
        }
    }
}
