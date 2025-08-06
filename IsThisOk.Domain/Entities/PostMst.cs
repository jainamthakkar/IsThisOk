using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IsThisOk.Domain.Entities
{
    public class PostMst
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string iUserId { get; set; }

        public string[] sImgUrls { get; set; }

        public string sContent { get; set; }

        public bool IsAnonymous { get; set; }

        public DateTime dCreatedAt { get; set; } = DateTime.UtcNow;

        public int iGreenFlagVotes { get; set; } = 0;
        public int iRedFlagVotes { get; set; } = 0;
    }
}
