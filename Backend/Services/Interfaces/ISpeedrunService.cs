using Shared.Models.DTOs;
using Shared.Models.Entities;

namespace Backend.Services.Interfaces;

public interface ISpeedrunService
{
    Task<List<Speedrun>> GetSpeedrunsAsync(SpeedrunRequest speedrunRequest);
    Task<List<Speedrun>> GetAllSpeedrunsAsync(Category category, Platform platform);
    Task<string> GetSpeedrunsJsonAsync(Category category, Platform platform);
}