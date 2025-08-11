using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FirebaseCounterController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _firebaseUrl;
        private readonly string _integerKey;

        public FirebaseCounterController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _firebaseUrl = configuration["Firebase:DatabaseUrl"]?.TrimEnd('/') + "/";
            _integerKey = configuration["Firebase:IntegerKey"] ?? "counter";
        }

        private string GetFullUrl() => $"{_firebaseUrl}{_integerKey}.json";

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var client = _httpClientFactory.CreateClient();
            var resp = await client.GetAsync(GetFullUrl());
            if (!resp.IsSuccessStatusCode)
                return StatusCode((int)resp.StatusCode, "Failed to GET from Firebase");

            var json = await resp.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json) || json == "null")
                return Ok(0);

            var val = JsonSerializer.Deserialize<int>(json);
            return Ok(val);
        }

        [HttpPost("increment")]
        public Task<IActionResult> Increment() => ChangeCounterBy(1);

        [HttpPost("decrement")]
        public Task<IActionResult> Decrement() => ChangeCounterBy(-1);

        private async Task<IActionResult> ChangeCounterBy(int delta)
        {
            var client = _httpClientFactory.CreateClient();

            // Get current value
            var getResp = await client.GetAsync(GetFullUrl());
            if (!getResp.IsSuccessStatusCode)
                return StatusCode((int)getResp.StatusCode, "Failed to GET current value from Firebase");

            var json = await getResp.Content.ReadAsStringAsync();
            int current = 0;
            if (!string.IsNullOrWhiteSpace(json) && json != "null")
            {
                current = JsonSerializer.Deserialize<int>(json);
            }

            int updated = current + delta;

            var content = new StringContent(JsonSerializer.Serialize(updated), Encoding.UTF8, "application/json");
            var putResp = await client.PutAsync(GetFullUrl(), content);
            if (!putResp.IsSuccessStatusCode)
                return StatusCode((int)putResp.StatusCode, "Failed to PUT updated value to Firebase");

            return Ok(updated);
        }
    }
}
