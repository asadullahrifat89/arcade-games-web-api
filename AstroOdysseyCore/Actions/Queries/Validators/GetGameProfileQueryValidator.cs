using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroOdysseyCore
{
    public class GetGameProfileQueryValidator : AbstractValidator<GetGameProfileQuery>
    {
        public GetGameProfileQueryValidator()
        {
            RuleFor(x => x.GameId).NotNull().NotEmpty();
            RuleFor(x => x.UserId).NotNull().NotEmpty();
        }
    }
}
