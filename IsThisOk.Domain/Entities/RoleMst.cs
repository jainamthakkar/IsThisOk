using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IsThisOk.Domain.Entities
{
    public class RoleMst
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string sRoleName { get; set; }

        public string sRoleDesc { get; set; }

        public DateTime dCreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime dModifiedOn { get; set; }

        public bool bIsActive { get; set; }

    }
}
