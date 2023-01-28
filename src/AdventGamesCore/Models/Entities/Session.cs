namespace AdventGamesCore
{
    public class Session : EntityBase
    {
        /// <summary>
        /// An encrypted session id that is sent to the client. Later this is validated server side by matching it's decrypted value with database.
        /// </summary>
        public string SessionId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;

        public string GameId { get; set; } = string.Empty;

        /// <summary>
        /// When a game play starts this is saved as false. Once score is submitted this is updated to true.
        /// </summary>
        public bool IsComplete { get; set; } = false;

        public static Session Initialize(GenerateSessionCommand command)
        {
            return new Session()
            {
                SessionId = DateTime.UtcNow.Ticks.ToString(),
                UserId = command.UserId,
                CompanyId = command.CompanyId,
                GameId = command.GameId,
                IsComplete = false,
            };
        }
    }
}