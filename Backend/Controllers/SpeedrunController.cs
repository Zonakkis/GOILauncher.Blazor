using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Apis.Speedruns;

[ApiController]
[Route("api/[controller]")]
public class SpeedrunController(ISpeedrunService speedrunService) : ControllerBase
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };
    [HttpGet("speedruns")]
    public async Task<IActionResult> GetSpeedruns()
    {
        var req = HttpContext.Request;
        var speedrunRequestJson = await new StreamReader(req.Body).ReadToEndAsync();
        if (!string.IsNullOrEmpty(speedrunRequestJson))
        {
            var speedrunRequest = JsonSerializer.Deserialize<SpeedrunRequest>(speedrunRequestJson,
                jsonSerializerOptions);
            if (speedrunRequest is not null)
            {
                var speedruns = await speedrunService.GetSpeedrunsAsync(speedrunRequest);
                return new OkObjectResult(speedruns);
            }
        }
        return new BadRequestObjectResult("Invalid speedrun request.");
    }

    [HttpGet("all-speedruns")]
    public async Task<IActionResult> GetAllSpeedruns()
    {
        var req = HttpContext.Request;
        if (req.Query.TryGetValue("category", out var categoryString)
            && Enum.TryParse<Category>(categoryString, out var category)
            && req.Query.TryGetValue("platform", out var platformString)
            && Enum.TryParse<Platform>(platformString, out var platform))
        {
            //var speedruns = await speedrunService.GetAllSpeedrunsAsync(category, platform);
            //return new OkObjectResult(speedruns);
            var speedrunsJson = await speedrunService.GetSpeedrunsJsonAsync(category, platform);
            return new OkObjectResult(speedrunsJson);
        }
        return new BadRequestObjectResult("Missing one or more parameters.");
    }
}