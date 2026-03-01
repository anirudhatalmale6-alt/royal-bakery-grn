using RoyalBakeryGrn.Models;
using RoyalBakeryGrn.Services;
using System.Collections.ObjectModel;

namespace RoyalBakeryGrn.Pages
{
    public partial class AdjustmentRequestsListPage : ContentPage
    {
        private readonly ApiClient _api;
        private List<AdjustmentDto> _allRequests = new();
        private ObservableCollection<AdjustmentDto> _filteredRequests = new();

        public AdjustmentRequestsListPage(ApiClient api)
        {
            InitializeComponent();
            _api = api;
            RequestsCollectionView.ItemsSource = _filteredRequests;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadRequests();
        }

        private async Task LoadRequests()
        {
            try
            {
                _allRequests = await _api.GetPendingAdjustmentsAsync();
                _filteredRequests.Clear();
                foreach (var r in _allRequests)
                    _filteredRequests.Add(r);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load requests: {ex.Message}", "OK");
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = e.NewTextValue?.ToLower() ?? string.Empty;

            var filtered = _allRequests.Where(r =>
                (!string.IsNullOrEmpty(r.GRNNumber) && r.GRNNumber.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(r.Reason) && r.Reason.ToLower().Contains(keyword))
            ).ToList();

            _filteredRequests.Clear();
            foreach (var item in filtered)
                _filteredRequests.Add(item);
        }

        private async void EnterCode_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button?.BindingContext is not AdjustmentDto request)
                return;

            string code = await DisplayPromptAsync("Admin Code", "Enter admin code for this request:");
            if (string.IsNullOrWhiteSpace(code))
                return;

            try
            {
                var result = await _api.ApproveAdjustmentAsync(request.Id, code.Trim());
                await DisplayAlert("Success", result, "OK");
                await LoadRequests();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void AddRequest_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditGRNPage(_api));
        }
    }
}
