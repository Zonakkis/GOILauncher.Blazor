using Backend.Repositories;
using Backend.Services.Interfaces;
using Shared.Models.DTOs;
using Shared.Models.Entities;

namespace Backend.Services
{
    internal class SpeedrunService(SpeedrunRepository speedrunRepository) : ISpeedrunService
    {
        public async Task<List<Speedrun>> GetSpeedrunsAsync(SpeedrunRequest speedrunRequest)
        {
            return await speedrunRepository.GetSpeedrunsAsync(
                speedrunRequest.Category,
                speedrunRequest.Platform,
                speedrunRequest.DateTo.ToUniversalTime(),
                speedrunRequest.Limit);
        }

        public async Task<List<Speedrun>> GetAllSpeedrunsAsync(Category category, Platform platform)
        {
            return await speedrunRepository.GetAllSpeedrunsAsync(category, platform);
        }
        public async Task<string> GetSpeedrunsJsonAsync(Category category, Platform platform)
        {
            return await speedrunRepository.GetSpeedrunsJsonAsync(category, platform);
        }
    }
}
