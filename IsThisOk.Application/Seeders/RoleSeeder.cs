using IsThisOk.Application.Seeders;
using IsThisOk.Domain.Entities;
using MongoDB.Driver;

public class RoleSeeder : IRoleSeeder
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<RoleMst> _rolesCollection;

    public RoleSeeder(IMongoDatabase database)
    {
        _database = database;
        _rolesCollection = _database.GetCollection<RoleMst>("Roles");
    }

    public async Task SeedDefaultRolesAsync()
    {
        var existingRoles = await _rolesCollection.Find(_ => true).ToListAsync();

        if (!existingRoles.Any())
        {
            var defaultRoles = new List<RoleMst>
            {
                new RoleMst
                {
                    sRoleName = "User",
                    sRoleDesc = "Regular anonymous user",
                    bIsActive = true,
                    dCreatedOn = DateTime.UtcNow,
                    dModifiedOn = DateTime.UtcNow
                },
                new RoleMst
                {
                    sRoleName = "Admin",
                    sRoleDesc = "Platform administrator",
                    bIsActive = true,
                    dCreatedOn = DateTime.UtcNow,
                    dModifiedOn = DateTime.UtcNow
                }
            };

            await _rolesCollection.InsertManyAsync(defaultRoles);
        }
    }
}