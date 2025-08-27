using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IsThisOk.Domain.Entities
{
    public enum AdminRequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class AdminRequestMst
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string sUserId { get; set; } = null!;           // Who wants to be admin?

        public string sRequestReason { get; set; } = null!;    // Why do they want admin access?

        [BsonRepresentation(BsonType.ObjectId)]
        public string? sApprovedByUserId { get; set; }         // Which SuperAdmin approved/rejected?

        public AdminRequestStatus eStatus { get; set; } = AdminRequestStatus.Pending;

        public string? sApprovalNotes { get; set; }            // SuperAdmin's notes

        public DateTime dRequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? dProcessedAt { get; set; }            // When was it approved/rejected?
    }
}