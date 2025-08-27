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
                },
                new RoleMst
                {
                sRoleName = "SuperAdmin",
                sRoleDesc = "Supreme administrator - only one exists",
                bIsActive = true,
                dCreatedOn = DateTime.UtcNow,
                dModifiedOn = DateTime.UtcNow
                }
            };

            await _rolesCollection.InsertManyAsync(defaultRoles);
        }
    }

    private async Task CreateSuperAdminIfNotExistsAsync()
    {
        var superAdminRole = await _rolesCollection
            .Find(r => r.sRoleName == "SuperAdmin")
            .FirstOrDefaultAsync();

        if (superAdminRole == null) return;

        var usersCollection = _database.GetCollection<UserMst>("Users");
        var existingSuperAdmin = await usersCollection
            .Find(u => u.iRoleId == superAdminRole.Id)
            .FirstOrDefaultAsync();

        // If no SuperAdmin exists, create one
        if (existingSuperAdmin == null)
        {
            var superAdmin = new UserMst
            {
                sUsername = "SuperAdmin",
                sEmail = "superadmin@isthisokay.com", // Change this to your email!
                sPasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@123"), // Change this password!
                iRoleId = superAdminRole.Id,
                sAnonymousDisplayName = "SuperAdmin", // SuperAdmin is not anonymous
                sGender = "Prefer not to say",
                bIsActive = true
            };

            await usersCollection.InsertOneAsync(superAdmin);

            // Log this important event
            Console.WriteLine("🚨 IMPORTANT: SuperAdmin created!");
            Console.WriteLine($"📧 Email: {superAdmin.sEmail}");
            Console.WriteLine($"🔑 Password: SuperAdmin@123");
            Console.WriteLine("⚠️  CHANGE THIS PASSWORD IMMEDIATELY!");
        }
    }
}