namespace AdventGamesCore
{
    public class UserProfile
    {
        public string UserId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public Dictionary<string, string> MetaData { get; set; } = new Dictionary<string, string>();

        public static UserProfile Initialize(User user)
        {
            return new UserProfile()
            {
                UserId = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                City = user.City,
                CompanyId = user.CompanyId,
                MetaData = user.MetaData,
                CreatedOn = user.CreatedOn,
            };
        }
    }
}
