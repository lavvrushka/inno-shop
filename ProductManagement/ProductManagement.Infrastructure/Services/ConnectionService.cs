using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Domain.Interfaces.IServices;
using System.Net.Http.Json;


namespace ProductManagement.Infrastructure.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly HttpClient _httpClient;

        public ConnectionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> UserExistsAndIsActiveAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"/api/user/{userId}/status");
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var status = await response.Content.ReadFromJsonAsync<UserStatusResponse>();
            return status?.IsActive ?? false;
        }
    }
}
