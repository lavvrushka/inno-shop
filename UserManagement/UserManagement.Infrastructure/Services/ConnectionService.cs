using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Infrastructure.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly HttpClient _httpClient;

        public ConnectionService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            if (_httpClient.BaseAddress == null)
            {
                throw new InvalidOperationException("BaseAddress не установлен в HttpClient.");
            }
        }


        public async Task<bool> HideUserProductsAsync(Guid userId)
        {
            var response = await _httpClient.PostAsync($"api/product/hide-by-user/{userId}", null);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> ShowUserProductsAsync(Guid userId)
        {
            var response = await _httpClient.PostAsync($"api/product/show-by-user/{userId}", null);

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteAllProductsByUserAsync(Guid userId)
        {
            var response = await _httpClient.DeleteAsync($"api/product/delete-all-by-user/{userId}");
            return response.IsSuccessStatusCode;
        }
    }
}
