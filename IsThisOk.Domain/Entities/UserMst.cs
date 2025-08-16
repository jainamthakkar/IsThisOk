using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IsThisOk.Domain.Entities
{
    public class UserMst
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string sUsername { get; set; }// Real username (private)

        public string sEmail { get; set; }// Real email (private)

        public string sPasswordHash { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string iRoleId { get; set; }


        // This is what others see: "Anonymous_Fox_123"
        public string sAnonymousDisplayName { get; set; }

        public bool bIsActive { get; set; } = true;

        public DateTime dCreatedAt { get; set; } = DateTime.UtcNow;

        public String sGender { get; set; }

    }
}
