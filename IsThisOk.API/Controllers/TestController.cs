using IsThisOk.Domain.Settings;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IMongoDatabase _database;

    public TestController(IMongoClient client, MongoDbSettings settings)
    {
        _database = client.GetDatabase(settings.DatabaseName);
    }

    [HttpGet]
    public IActionResult Get()
    {
        var collections = _database.ListCollectionNames().ToList();
        return Ok(new { Message = "MongoDB Connected!!", Collections = collections });
    }

    [HttpGet("sample")]
    public IActionResult testt()
    {
        return Ok(new { Message = "API is working!" });
    }
}
