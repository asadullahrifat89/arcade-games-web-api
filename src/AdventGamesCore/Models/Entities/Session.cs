namespace AdventGamesCore
{
    public class Session : EntityBase
    {
        /// <summary>
        /// An encrypted session id that is sent to the client. Later this is validated server side by matching it's decrypted value with database.
        /// </summary>
        public string SessionId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public DateTime ExpiresOn { get; set; }

        public string GameId { get; set; } = string.Empty;

        public static Session Initialize(GenerateSessionCommand command)
        {
            return new Session()
            {
                ExpiresOn = DateTime.UtcNow.AddDays(2),
                GameId = command.GameId,
                UserId = command.UserId,
                SessionId = DateTime.UtcNow.Ticks.ToString(),
            };
        }
    }
}