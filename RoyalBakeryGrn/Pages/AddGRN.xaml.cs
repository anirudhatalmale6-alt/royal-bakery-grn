using RoyalBakeryGrn.Models;
using RoyalBakeryGrn.Services;
using System.Collections.ObjectModel;

namespace RoyalBakeryGrn.Pages
{
    public partial class AddGRN : ContentPage
    {
        private readonly ApiClient _api;
        private List<MenuItemDto> _menuItems = new();
        private ObservableCollection<GrnItemViewModel> GRNItems { get; set; } = new();

        public AddGRN(ApiClient api)
        {
            InitializeComponent();
            _api = api;
            GRNListView.ItemsSource = GRNItems;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadMenuItems();
        }

        private async Task LoadMenuItems()
        {
            try
            {
                _menuItems = await _api.GetMenuItemsAsync();
                ItemPicker.ItemsSource = _menuItems;
                ItemPicker.ItemDisplayBinding = new Binding("Name");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load menu items: {ex.Message}", "OK");
            }
        }

        private void AddToGRN_Clicked(object sender, EventArgs e)
        {
            var selectedItem = ItemPicker.SelectedItem as MenuItemDto;
            if (selectedItem == null)
            {
                DisplayAlert("Error", "Please select an item.", "OK");
                return;
            }

            if (!int.TryParse(QuantityEntry.Text, out int quantity) || quantity <= 0)
            {
                DisplayAlert("Error", "Please enter a valid quantity.", "OK");
                return;
            }

            var existing = GRNItems.FirstOrDefault(x => x.MenuItemId == selectedItem.Id);
            if (existing != null)
            {
                existing.Quantity += quantity;
                // Refresh binding
                var idx = GRNItems.IndexOf(existing);
                GRNItems[idx] = existing;
            }
            else
            {
                GRNItems.Add(new GrnItemViewModel
                {
                    MenuItemId = selectedItem.Id,
                    ItemName = selectedItem.Name,
                    Quantity = quantity,
                    Price = selectedItem.Price
                });
            }

            QuantityEntry.Text = string.Empty;
            ItemPicker.SelectedIndex = -1;
        }

        private void RemoveGRNItem_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is GrnItemViewModel item)
            {
                GRNItems.Remove(item);
            }
        }

        private void QuantityEntry_Completed(object sender, EventArgs e)
        {
            if (sender is Entry entry && entry.BindingContext is GrnItemViewModel item)
            {
                if (int.TryParse(entry.Text, out int newQty) && newQty > 0)
                {
                    item.Quantity = newQty;
                }
                else
                {
                    entry.Text = item.Quantity.ToString();
                }
            }
        }

        private async void SaveGRN_Clicked(object sender, EventArgs e)
        {
            if (GRNItems.Count == 0)
            {
                await DisplayAlert("Error", "No items to save.", "OK");
                return;
            }

            try
            {
                var request = new CreateGrnRequest
                {
                    Items = GRNItems.Select(i => new CreateGrnItemRequest
                    {
                        MenuItemId = i.MenuItemId,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                };

                var result = await _api.CreateGrnAsync(request);
                await DisplayAlert("Success", $"GRN {result.GRNNumber} saved!", "OK");
                GRNItems.Clear();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    // Simple view model for GRN item display
    public class GrnItemViewModel
    {
        public int MenuItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
