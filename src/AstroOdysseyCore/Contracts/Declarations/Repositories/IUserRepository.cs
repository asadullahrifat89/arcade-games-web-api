namespace AstroOdysseyCore
{
    public interface IUserRepository
    {
        Task<bool> BeAnExistingUser(string id);

        Task<bool> BeAnExistingUserEmail(string userEmail);

        Task<bool> BeAnExistingUserName(string userName);

        Task<bool> BeAnExistingUserNameOrEmail(string userNameOrEmail);

        Task<bool> BeValidUser(string userNameOrEmail, string password);

        Task<QueryRecordResponse<User>> GetUser(GetUserQuery query);

        Task<ActionCommandResponse> Signup(SignupCommand command);
    }
}
