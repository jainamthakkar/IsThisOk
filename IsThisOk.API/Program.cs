using IsThisOk.Application.Seeders;
using IsThisOk.Domain.Settings;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Bind and register MongoDbSettings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(nameof(MongoDbSettings)));

var mongoDbSettings = builder.Configuration
    .GetSection(nameof(MongoDbSettings))
    .Get<MongoDbSettings>();

builder.Services.AddSingleton(mongoDbSettings);

// Register MongoClient
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(mongoDbSettings.ConnectionString));

// Register IMongoDatabase here
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<MongoDbSettings>();
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

builder.Services.AddSingleton<IRoleSeeder, RoleSeeder>();

// Add Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

//// Middleware
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IRoleSeeder>();
    await seeder.SeedDefaultRolesAsync();
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
