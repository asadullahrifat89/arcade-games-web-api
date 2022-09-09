using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroOdysseyCore
{
    public class SignupCommandValidator : AbstractValidator<SignupCommand>
    {
        private readonly IUserRepository _userRepository;

        public SignupCommandValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.UserName).NotNull().NotEmpty();
            RuleFor(x => x.UserName).MustAsync(NotBeAnExistingUserName).WithMessage("Username already exists.");

            RuleFor(x => x.Email).NotNull().NotEmpty();
            RuleFor(x => x.Email).MustAsync(NotBeAnExistingUserEmail).WithMessage("Email already exists.");

            RuleFor(x => x.Password).NotNull().NotEmpty();
            RuleFor(x => x.GameId).NotNull().NotEmpty();
        }

        private async Task<bool> NotBeAnExistingUserName(string userName, CancellationToken arg2)
        {
            return !await _userRepository.BeAnExistingUserName(userName);
        }

        private async Task<bool> NotBeAnExistingUserEmail(string userEmail, CancellationToken arg2)
        {
            return !await _userRepository.BeAnExistingUserEmail(userEmail);
        }
    }
}
