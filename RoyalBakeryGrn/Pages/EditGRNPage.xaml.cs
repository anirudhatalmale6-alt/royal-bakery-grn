using RoyalBakeryGrn.Models;
using RoyalBakeryGrn.Services;

namespace RoyalBakeryGrn.Pages
{
    public partial class EditGRNPage : ContentPage
    {
        private readonly ApiClient _api;
        private List<GrnDto> _grns = new();
        private GrnDto? _selectedGRN;
        private List<GrnItemDto> _editableItems = new();

        public EditGRNPage(ApiClient api)
        {
            InitializeComponent();
            _api = api;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadGRNs();
        }

        private async Task LoadGRNs()
        {
            try
            {
                _grns = await _api.GetGrnsAsync();
                GRNPicker.ItemsSource = _grns;
                GRNPicker.ItemDisplayBinding = new Binding("GRNNumber");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load GRNs: {ex.Message}", "OK");
            }
        }

        private void LoadGRNItems_Clicked(object sender, EventArgs e)
        {
            if (GRNPicker.SelectedItem is not GrnDto grn)
            {
                DisplayAlert("Error", "Select a GRN first", "OK");
                return;
            }

            _selectedGRN = grn;
            _editableItems = grn.Items.Select(i => new GrnItemDto
            {
                Id = i.Id,
                MenuItemId = i.MenuItemId,
                ItemName = i.ItemName,
                Quantity = i.Quantity,
                Price = i.Price,
                CurrentQuantity = i.CurrentQuantity
            }).ToList();

            AdjustmentItemsView.ItemsSource = null;
            AdjustmentItemsView.ItemsSource = _editableItems;
        }

        private void RemoveAdjustmentItem_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is GrnItemDto item)
            {
                _editableItems.Remove(item);
                AdjustmentItemsView.ItemsSource = null;
                AdjustmentItemsView.ItemsSource = _editableItems;
            }
        }

        private async void SaveAdjustmentRequest_Clicked(object sender, EventArgs e)
        {
            if (_selectedGRN == null)
            {
                await DisplayAlert("Error", "Load GRN first", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(ReasonEntry.Text))
            {
                await DisplayAlert("Error", "Enter a reason", "OK");
                return;
            }

            try
            {
                var request = new CreateAdjustmentRequest
                {
                    GRNId = _selectedGRN.Id,
                    Reason = ReasonEntry.Text.Trim(),
                    Items = _editableItems.Select(i => new AdjustmentItemDto
                    {
                        MenuItemId = i.MenuItemId,
                        ItemName = i.ItemName,
                        RequestedQuantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                };

                var result = await _api.CreateAdjustmentAsync(request);
                await DisplayAlert("Success", $"Adjustment request saved. Admin code: {result.AdminCode}", "OK");
                AdjustmentItemsView.ItemsSource = null;
                ReasonEntry.Text = string.Empty;
                _selectedGRN = null;
                _editableItems.Clear();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
