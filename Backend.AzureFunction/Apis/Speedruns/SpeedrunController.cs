using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Shared.Models.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Apis.Speedruns;

public class SpeedrunController(ISpeedrunService speedrunService, ILogger<SpeedrunController> logger)
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };
    [Function("GetSpeedruns")]
    public async Task<IActionResult> GetSpeedruns(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "speedruns")] HttpRequest req)
    {
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

    [Function("GetAllSpeedruns")]
    public async Task<IActionResult> GetAllSpeedruns(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "all-speedruns")] HttpRequest req)
    {
        //if (req.Query.TryGetValue("category", out var category)
        //    && req.Query.TryGetValue("platform", out var platform))
        //{
        //    var speedruns = await speedrunService.GetAllSpeedrunsAsync(
        //        Enum.Parse<Category>(category), Enum.Parse<Platform>(platform));
        //    //return new OkObjectResult(speedruns);
        //    return new OkObjectResult("³É¹¦£¡");
        //}
        return new BadRequestObjectResult("Missing one or more parameters.");
    }
}