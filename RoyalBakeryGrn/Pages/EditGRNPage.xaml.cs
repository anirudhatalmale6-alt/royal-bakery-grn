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

        private async void LoadGRNItems_Clicked(object sender, EventArgs e)
        {
            if (GRNPicker.SelectedItem is not GrnDto grn)
            {
                await DisplayAlert("Error", "Select a GRN first", "OK");
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

            // Load today's edit history for this GRN
            await LoadEditHistory(grn.Id);
        }

        private async Task LoadEditHistory(int grnId)
        {
            try
            {
                var edits = await _api.GetGrnEditsAsync(grnId);
                if (edits.Any())
                {
                    EditHistoryView.ItemsSource = edits;
                    EditHistoryFrame.IsVisible = true;
                }
                else
                {
                    EditHistoryFrame.IsVisible = false;
                }
            }
            catch
            {
                EditHistoryFrame.IsVisible = false;
            }
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

        private async void SaveDirectEdit_Clicked(object sender, EventArgs e)
        {
            if (_selectedGRN == null)
            {
                await DisplayAlert("Error", "Load GRN first", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(ReasonEntry.Text))
            {
                await DisplayAlert("Error", "Enter a reason for the edit", "OK");
                return;
            }

            // Client-side validation: cannot reduce below sold quantity
            foreach (var item in _editableItems)
            {
                var original = _selectedGRN.Items.FirstOrDefault(o => o.MenuItemId == item.MenuItemId);
                if (original != null)
                {
                    int soldQty = original.Quantity - original.CurrentQuantity;
                    if (item.Quantity < soldQty)
                    {
                        await DisplayAlert("Error",
                            $"Cannot set {item.ItemName} quantity to {item.Quantity}.\n\n" +
                            $"{soldQty} units have already been sold. Minimum quantity is {soldQty}.",
                            "OK");
                        return;
                    }
                }
            }

            try
            {
                var request = new DirectEditGrnRequest
                {
                    Reason = ReasonEntry.Text.Trim(),
                    Items = _editableItems.Select(i => new AdjustmentItemDto
                    {
                        MenuItemId = i.MenuItemId,
                        ItemName = i.ItemName,
                        RequestedQuantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                };

                var result = await _api.DirectEditGrnAsync(_selectedGRN.Id, request);
                await DisplayAlert("Success", result, "OK");

                // Refresh GRN data and edit history
                await LoadGRNs();
                ReasonEntry.Text = string.Empty;

                // Reload the same GRN to show updated items + edit history
                var refreshed = _grns.FirstOrDefault(g => g.Id == _selectedGRN.Id);
                if (refreshed != null)
                {
                    _selectedGRN = refreshed;
                    _editableItems = refreshed.Items.Select(i => new GrnItemDto
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
                    await LoadEditHistory(_selectedGRN.Id);
                }
                else
                {
                    AdjustmentItemsView.ItemsSource = null;
                    _selectedGRN = null;
                    _editableItems.Clear();
                    EditHistoryFrame.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
