using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IsThisOk.Domain.Entities
{
    public class UserMst
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string sUsername { get; set; }

        public string sEmail { get; set; }

        public string sPasswordHash { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string iRoleId { get; set; }

        public bool IsAnonymous { get; set; }

        public DateTime dCreatedAt { get; set; } = DateTime.UtcNow;

        public String sGender { get; set; }

    }
}
