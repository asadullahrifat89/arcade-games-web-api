﻿using AdventGamesCore.Extensions;
using FluentValidation;

namespace AdventGamesCore
{
    public class GetGameWinnersQueryValidator : AbstractValidator<GetGameWinnersQuery>
    {
        private readonly ICompanyRepository _companyRepository;

        public GetGameWinnersQueryValidator(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;

            RuleFor(x => x.GameId).NotNull().NotEmpty();
            RuleFor(x => x.GameId).Must(x => Constants.GAME_IDS.Contains(x)).WithMessage("Invalid game id.");

            RuleFor(x => x.Filter).Must(x => Enum.GetValues<HighScoreFilter>().Contains(x)).WithMessage("Invalid filter.");

            RuleFor(x => x.FromDate).NotNull().When(x => x.Filter == HighScoreFilter.DATE || x.Filter == HighScoreFilter.DATERANGE).WithMessage("From date cannot be null.");
            RuleFor(x => x.ToDate).NotNull().When(x => x.Filter == HighScoreFilter.DATERANGE).WithMessage("To date cannot be null.");

            RuleFor(x => x).Must(x => x.FromDate <= x.ToDate).WithMessage("Invalid date range.").When(x => x.FromDate != DateTime.MinValue && x.ToDate != DateTime.MinValue);

            RuleFor(x => x.Limit).GreaterThan(0);

            RuleFor(x => x.CompanyId).NotNull().NotEmpty();
            RuleFor(x => x.CompanyId).MustAsync(BeAnExistingCompany).WithMessage("Company doesn't exist.").When(x => !x.CompanyId.IsNullOrBlank());
        }

        private async Task<bool> BeAnExistingCompany(string userName, CancellationToken arg2)
        {
            return await _companyRepository.BeAnExistingCompany(userName);
        }
    }
}
