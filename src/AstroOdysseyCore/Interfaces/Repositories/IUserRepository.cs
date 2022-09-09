namespace AstroOdysseyCore
{
    public interface IUserRepository
    {
        Task<bool> BeAnExistingUserEmail(string userEmail);

        Task<bool> BeAnExistingUserName(string userName);

        Task<bool> BeAnExistingUserNameOrEmail(string userNameOrEmail);

        Task<bool> BeValidUser(string userNameOrEmail, string password);

        Task<GameProfile> Signup(SignupCommand command);
    }
}
