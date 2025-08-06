using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IsThisOk.Domain.Entities
{
    public class CommentMst
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string iPostId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string iUserId { get; set; }

        public string sCommentText { get; set; }

        public bool IsAnonymous { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}