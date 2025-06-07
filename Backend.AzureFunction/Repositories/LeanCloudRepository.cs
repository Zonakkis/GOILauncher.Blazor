using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Backend.Constants;
using Backend.Reposties.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.Models.DTOs.LeanCloud;

namespace Backend.Reposties
{
    public class LeanCloudRepository<T>(IHttpClientFactory httpClientFactory,
        ILogger<LeanCloudRepository<T>> logger) : ILeanCloudRepository<T>
    {
        private readonly string _keys = string.Join(',',
            from property in typeof(T).GetProperties()
            let attribute = property.GetCustomAttribute<JsonPropertyNameAttribute>(false)
            where attribute is not null
            select attribute.Name);

        public async Task<T?> GetAsync(string id)
        {
            var httpClient = httpClientFactory.CreateClient(LeanCloudConstants.HttpClientName);
            var httpResponseMessage = await httpClient.GetAsync($"{typeof(T).Name}/{id}");
            var json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<T>(json);
            var error = JsonSerializer.Deserialize<ErrorResponse>(json);
            logger.LogError($"[{error?.Code}]: {error?.Error}");
            return default;
        }

        public async Task<List<T>> ListAsync()
        {
            var httpClient = httpClientFactory.CreateClient(LeanCloudConstants.HttpClientName);
            var parameters = new Dictionary<string, string>
            {
                { "keys", _keys }
            };
            var parameterString = string.Join('&',
                from parameter in parameters
                let value = HttpUtility.UrlEncode(parameter.Value)
                select $"{parameter.Key}={value}");
            var requestUri = $"{typeof(T).Name}?{parameterString}";
            var httpResponseMessage = await httpClient.GetAsync(requestUri);
            var json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var queryResponse = JsonSerializer.Deserialize<QueryResponse<T>>(json);
                return queryResponse?.Results ?? [];
            }
            var error = JsonSerializer.Deserialize<ErrorResponse>(json);
            logger.LogError($"[{error?.Code}]: {error?.Error}");
            return [];
        }
    }
}
