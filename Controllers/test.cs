using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class RedisTestController : ControllerBase
{
    private readonly IDatabase _redisDb;

    public RedisTestController(IDatabase redisDb)
    {
        _redisDb = redisDb;
    }

    [HttpGet("test")]
    public async Task<IActionResult> TestRedis()
    {
        // Set a test value
        await _redisDb.StringSetAsync("foo", "bar");

        // Get the test value
        var result = await _redisDb.StringGetAsync("foo");

        return Ok(new { message = result.ToString() });
    }
}