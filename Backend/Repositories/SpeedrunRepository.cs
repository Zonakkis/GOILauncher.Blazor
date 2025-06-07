using Backend.Constants;
using Backend.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Models.DTOs;
using Shared.Models.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            var speedruns = await (from speedrun in dbContext.Speedruns.AsNoTracking()
                                   where speedrun.Category == category.ToString()
                                   && (platform == Platform.Any || speedrun.Platform == platform.ToString())
                                   orderby speedrun.Date
                                   select speedrun).ToListAsync();
            if (!(from speedrunJson in dbContext.SpeedrunsJson
                  where speedrunJson.Category == category
                  && speedrunJson.Platform == platform
                  select speedrunJson).Any())
            {
                dbContext.SpeedrunsJson.Add(new SpeedrunsJson
                {
                    Category = category,
                    Platform = platform,
                    Json = JsonSerializer.Serialize(speedruns, new JsonSerializerOptions
                    {
                        WriteIndented = false,
                        Converters = { new JsonStringEnumConverter() }
                    })
                });
                await dbContext.SaveChangesAsync();
            }
            //using var connection = new NpgsqlConnection(SQLConstants.ConnectionString);
            //await connection.OpenAsync();
            //var sql = "SELECT * FROM speedruns";
            //var parameters = new { Category = category.ToString(), Platform = platform.ToString() };
            //return [.. await connection.QueryAsync<Speedrun>(sql, parameters)];
            return speedruns;
            //return await (from speedrun in dbContext.Speedruns.AsNoTracking()
            //              where speedrun.Category == category.ToString()
            //              && (platform == Platform.Any || speedrun.Platform == platform.ToString())
            //              orderby speedrun.Date
            //              select speedrun).ToListAsync();
        }

        public async Task<string> GetSpeedrunsJsonAsync(Category category, Platform platform)
        {
            using var connection = new NpgsqlConnection(SQLConstants.ConnectionString);
            await connection.OpenAsync();
            var sql = @"SELECT * 
                        FROM speedruns_json
                        WHERE category = @Category AND platform = @Platform";
            var parameters = new { Category = category.ToString(), Platform = platform.ToString() };
            return (await connection.QueryFirstAsync<SpeedrunsJson>(sql, parameters)).Json;
            //return (await (from speedrunJson in dbContext.SpeedrunsJson.AsNoTracking()
            //               where speedrunJson.Category == category
            //               && speedrunJson.Platform == platform
            //               select new
            //               {
            //                   speedrunJson.Json
            //               }).FirstAsync())?.Json ?? string.Empty;
        }
    }
}
