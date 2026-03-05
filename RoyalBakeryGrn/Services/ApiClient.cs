using System.Net.Http.Json;
using System.Text.Json;
using RoyalBakeryGrn.Models;

namespace RoyalBakeryGrn.Services
{
    public class ApiClient
    {
        private HttpClient _http;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private string _baseUrl = "";

        public string BaseUrl
        {
            get => _baseUrl;
            set
            {
                _baseUrl = value.TrimEnd('/');
                _http = CreateHttpClient();
                _http.BaseAddress = new Uri(_baseUrl + "/");
            }
        }

        public ApiClient()
        {
            _http = CreateHttpClient();

            // Load saved URL
            var saved = Preferences.Get("api_base_url", "");
            if (!string.IsNullOrEmpty(saved))
            {
                _baseUrl = saved.TrimEnd('/');
                _http.BaseAddress = new Uri(_baseUrl + "/");
            }
        }

        private static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
            return new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(15) };
        }

        public bool IsConfigured => _http.BaseAddress != null;

        // ===== Auth =====
        public async Task<LoginResponse> LoginAsync(string username, string password)
        {
            var resp = await _http.PostAsJsonAsync("api/auth/login",
                new LoginRequest { Username = username, Password = password }, _jsonOptions);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                try
                {
                    var err = JsonSerializer.Deserialize<ApiError>(body, _jsonOptions);
                    throw new Exception(err?.Message ?? "Login failed");
                }
                catch (JsonException)
                {
                    throw new Exception(body);
                }
            }
            return await resp.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)
                ?? throw new Exception("Empty response from server");
        }

        // ===== Menu =====
        public async Task<List<MenuItemDto>> GetMenuItemsAsync()
        {
            var resp = await _http.GetAsync("api/menu/items");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<List<MenuItemDto>>(_jsonOptions) ?? new();
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var resp = await _http.GetAsync("api/menu/categories");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<List<CategoryDto>>(_jsonOptions) ?? new();
        }

        // ===== Stock =====
        public async Task<List<StockDto>> GetStockAsync()
        {
            var resp = await _http.GetAsync("api/stock");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<List<StockDto>>(_jsonOptions) ?? new();
        }

        // ===== GRN =====
        public async Task<List<GrnDto>> GetGrnsAsync()
        {
            var resp = await _http.GetAsync("api/grn");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<List<GrnDto>>(_jsonOptions) ?? new();
        }

        public async Task<GrnDto?> GetGrnByIdAsync(int id)
        {
            var resp = await _http.GetAsync($"api/grn/{id}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<GrnDto>(_jsonOptions);
        }

        public async Task<GrnDto> CreateGrnAsync(CreateGrnRequest request)
        {
            var resp = await _http.PostAsJsonAsync("api/grn", request, _jsonOptions);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create GRN: {error}");
            }
            return await resp.Content.ReadFromJsonAsync<GrnDto>(_jsonOptions)
                ?? throw new Exception("Empty response from server");
        }

        // ===== Adjustments =====
        public async Task<List<AdjustmentDto>> GetPendingAdjustmentsAsync()
        {
            var resp = await _http.GetAsync("api/adjustment");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<List<AdjustmentDto>>(_jsonOptions) ?? new();
        }

        public async Task<AdjustmentDto> CreateAdjustmentAsync(CreateAdjustmentRequest request)
        {
            var resp = await _http.PostAsJsonAsync("api/adjustment", request, _jsonOptions);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create adjustment: {error}");
            }
            return await resp.Content.ReadFromJsonAsync<AdjustmentDto>(_jsonOptions)
                ?? throw new Exception("Empty response from server");
        }

        public async Task<string> ApproveAdjustmentAsync(int id, string adminCode)
        {
            var resp = await _http.PostAsJsonAsync($"api/adjustment/{id}/approve",
                new ApproveAdjustmentRequest { AdminCode = adminCode }, _jsonOptions);
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                try
                {
                    var err = JsonSerializer.Deserialize<ApiError>(body, _jsonOptions);
                    throw new Exception(err?.Message ?? body);
                }
                catch (JsonException)
                {
                    throw new Exception(body);
                }
            }
            return "Changes applied and GRN updated!";
        }

        // ===== Direct GRN Edit =====
        public async Task<string> DirectEditGrnAsync(int grnId, DirectEditGrnRequest request)
        {
            var resp = await _http.PutAsJsonAsync($"api/grn/{grnId}", request, _jsonOptions);
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                try
                {
                    var err = JsonSerializer.Deserialize<ApiError>(body, _jsonOptions);
                    throw new Exception(err?.Message ?? body);
                }
                catch (JsonException)
                {
                    throw new Exception(body);
                }
            }
            try
            {
                var result = JsonSerializer.Deserialize<ApiError>(body, _jsonOptions);
                return result?.Message ?? "GRN updated successfully";
            }
            catch { return "GRN updated successfully"; }
        }

        public async Task<List<GrnEditLogDto>> GetGrnEditsAsync(int grnId)
        {
            var resp = await _http.GetAsync($"api/grn/{grnId}/edits");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<List<GrnEditLogDto>>(_jsonOptions) ?? new();
        }

        // ===== Clearance =====
        public async Task<List<ClearanceDto>> GetTodayClearancesAsync()
        {
            var resp = await _http.GetAsync("api/clearance");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<List<ClearanceDto>>(_jsonOptions) ?? new();
        }

        public async Task<ClearanceDto> CreateClearanceAsync(CreateClearanceRequest request)
        {
            var resp = await _http.PostAsJsonAsync("api/clearance", request, _jsonOptions);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                try
                {
                    var err = JsonSerializer.Deserialize<ApiError>(body, _jsonOptions);
                    throw new Exception(err?.Message ?? body);
                }
                catch (JsonException)
                {
                    throw new Exception(body);
                }
            }
            return await resp.Content.ReadFromJsonAsync<ClearanceDto>(_jsonOptions)
                ?? throw new Exception("Empty response from server");
        }
    }
}
