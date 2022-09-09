using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace AstroOdysseyCore
{
    [BsonIgnoreExtraElements]
    public class EntityBase
    {
        [BsonId]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedOn { get; set; } = null;
    }
}
