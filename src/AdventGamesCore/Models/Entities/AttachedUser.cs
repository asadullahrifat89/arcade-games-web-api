using MongoDB.Bson.Serialization.Attributes;

namespace AdventGamesCore
{
    [BsonIgnoreExtraElements]
    public class AttachedUser
    {
        public string UserId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
    }
}
