namespace AdventGamesCore
{
    public class SignupCommand : RequestBase<ServiceResponse>
    {
        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;

        public Dictionary<string, string> MetaData { get; set; } = new Dictionary<string, string>();
    }
}
