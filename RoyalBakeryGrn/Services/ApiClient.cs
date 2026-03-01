using System.Net.Http.Json;
using System.Text.Json;
using RoyalBakeryGrn.Models;

namespace RoyalBakeryGrn.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public string BaseUrl
        {
            get => _http.BaseAddress?.ToString().TrimEnd('/') ?? "";
            set => _http.BaseAddress = new Uri(value.TrimEnd('/') + "/");
        }

        public ApiClient()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
            _http = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(15) };

            // Load saved URL
            var saved = Preferences.Get("api_base_url", "");
            if (!string.IsNullOrEmpty(saved))
                _http.BaseAddress = new Uri(saved.TrimEnd('/') + "/");
        }

        public bool IsConfigured => _http.BaseAddress != null;

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
