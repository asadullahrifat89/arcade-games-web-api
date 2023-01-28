namespace AdventGamesCore
{
    public interface IUserRepository
    {
        Task<bool> BeAnExistingUser(string id);

        Task<bool> BeAnExistingUserEmail(string userEmail, string companyId);

        Task<bool> BeAnExistingUserName(string userName, string companyId);

        Task<bool> BeAnExistingUserNameOrEmail(string userNameOrEmail, string companyId = "");

        Task<bool> BeValidUser(string userNameOrEmail, string password);

        Task<User> GetUser(string userNameOrEmail, string password, string companyId);

        Task<QueryRecordResponse<UserProfile>> GetUserProfile(GetUserProfileQuery query);

        Task<QueryRecordsResponse<UserProfile>> GetUserProfiles(GetUserProfilesQuery query);

        Task<ServiceResponse> Signup(SignupCommand command);

        Task<User[]> GetUsers(string[] userIds, string companyId);

        Task<User> GetUser(string userId, string companyId);
    }
}
