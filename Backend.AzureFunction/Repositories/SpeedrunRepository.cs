using Backend.Constants;
using Backend.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Models.DTOs;
using Shared.Models.Entities;

namespace Backend.Repositories
{
    public class SpeedrunRepository(SupaBaseDbContext dbContext)
    {
        public async Task<List<Speedrun>> GetSpeedrunsAsync(Category category, Platform platform,
            DateTime dateTo, int limit)
        {
            var query =
                from speedrun in dbContext.Speedruns
                where speedrun.Date <= dateTo
                      && speedrun.Category == category.ToString()
                      && (platform == Platform.Any || speedrun.Platform == platform.ToString())
                group speedrun by speedrun.Player into g
                select new
                {
                    Player = g.Key,
                    MinTime = g.Min(r => r.Time)
                };

            return await (
                from run in dbContext.Speedruns
                join q in query on new { run.Player, run.Time } equals new { q.Player, Time = q.MinTime }
                orderby run.Time
                select run
            ).Take(limit).ToListAsync();
        }

        public async Task<List<Speedrun>> GetAllSpeedrunsAsync(Category category, Platform platform)
        {
            using var connection = new NpgsqlConnection(SQLConstants.ConnectionString);
            await connection.OpenAsync();
            var sql = @"SELECT * 
                        FROM speedruns 
                        WHERE category = @category 
                        AND (platform = 'Any' OR @platform = @platform) 
                        ORDER BY date;";
            var parameters = new { Category = category.ToString(), Platform = platform.ToString() };
            return [.. await connection.QueryAsync<Speedrun>(sql, parameters)];
            //return await dbContext.Speedruns
            //    .AsNoTracking()
            //    .ToListAsync();
            //return await (from speedrun in dbContext.Speedruns.AsNoTracking()
            //              where speedrun.Category == category
            //              && (platform == Platform.Any || speedrun.Platform == platform)
            //              orderby speedrun.Date
            //              select speedrun).ToListAsync();
        }
    }
}
