using FluentValidation;

namespace AstroOdysseyCore
{
    public class AuthenticationCommandValidator : AbstractValidator<AuthenticationCommand>
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationCommandValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.UserName).NotNull().NotEmpty();
            RuleFor(x => x.Password).NotNull().NotEmpty();
            

            //TODO: check if user exists
        }
    }
}
