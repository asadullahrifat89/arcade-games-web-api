namespace AstroOdysseyCore
{
    public interface IUserRepository
    {
        Task<bool> BeAnExistingUser(string email);

        Task<bool> BeValidUser(string email, string password);

        Task<GameProfile> Signup(SignupCommand command);
    }
}
