namespace AdventGamesCore
{
    public class SubmitGameScoreCommand : RequestBase<ServiceResponse>
    {
        public AttachedUser User { get; set; } = new AttachedUser();

        public double Score { get; set; } = 0;

        public string CompanyId { get; set; } = string.Empty;

        public string SessionId { get; set; } = string.Empty;
    }

    public class SubmitGameScoreCommandDto : RequestBaseDto
    {
        public AttachedUser User { get; set; } = new AttachedUser();

        public double Score { get; set; } = 0;

        public string SessionId { get; set; } = string.Empty;
    }
}
