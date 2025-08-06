using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IsThisOk.Domain.Entities
{
    public class VoteMst
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string iPostId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string iUserId { get; set; }

        public string sVoteType { get; set; } // "Red" or "Green"

        public DateTime dCreatedAt { get; set; } = DateTime.UtcNow;
    }
}
