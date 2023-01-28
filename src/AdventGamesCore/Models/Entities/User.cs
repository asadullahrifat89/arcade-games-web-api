using AdventGamesCore.Extensions;

namespace AdventGamesCore
{
    public class User : EntityBase
    {
        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;

        public Dictionary<string, string> MetaData { get; set; } = new Dictionary<string, string>();

        public static User Initialize(SignupCommand command)
        {
            var encryptedPassword = command.Password.Encrypt();

            var user = new User()
            {
                FullName = command.FullName,
                UserName = command.UserName,
                Email = command.Email,
                Password = encryptedPassword,
                City = command.City,
                MetaData = command.MetaData,
                CompanyId = command.CompanyId,
            };

            return user;
        }
    }
}
