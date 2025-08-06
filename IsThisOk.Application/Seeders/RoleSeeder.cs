using IsThisOk.Domain.Entities;
using MongoDB.Driver;

namespace IsThisOk.Application.Seeders
{
    public class RoleSeeder : IRoleSeeder
    {
        private readonly IMongoCollection<RoleMst> _roleCollection;

        public RoleSeeder(IMongoDatabase database)
        {
            _roleCollection = database.GetCollection<RoleMst>("RoleMst");
        }

        public async Task SeedDefaultRolesAsync()
        {
            var existingRoles = await _roleCollection.Find(_ => true).ToListAsync();
            if (existingRoles.Any()) return; // Roles already seeded

            var roles = new List<RoleMst>
            {
                new RoleMst
                {
                    sRoleName = "User",
                    sRoleDesc = "Regular user",
                    bIsActive = true,
                    dCreatedOn = DateTime.UtcNow
                },
                new RoleMst
                {
                    sRoleName = "Admin",
                    sRoleDesc = "System administrator",
                    bIsActive = true,
                    dCreatedOn = DateTime.UtcNow
                },
                new RoleMst
                {
                    sRoleName = "Moderator",
                    sRoleDesc = "Can moderate posts",
                    bIsActive = true,
                    dCreatedOn = DateTime.UtcNow
                },
                new RoleMst
                {
                    sRoleName = "Anonymous",
                    sRoleDesc = "Hidden identity poster",
                    bIsActive = true,
                    dCreatedOn = DateTime.UtcNow
                }
            };

            await _roleCollection.InsertManyAsync(roles);
        }
    }
}
