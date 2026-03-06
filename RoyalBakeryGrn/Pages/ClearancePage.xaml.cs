using RoyalBakeryGrn.Models;
using RoyalBakeryGrn.Services;
using System.Collections.ObjectModel;

namespace RoyalBakeryGrn.Pages
{
    public partial class ClearancePage : ContentPage
    {
        private readonly ApiClient _api;
        private ObservableCollection<ClearanceDto> _todayClearances = new();
        private List<MenuItemDto> _allMenuItems = new();
        private MenuItemDto? _selectedMenuItem;
        private bool _suppressSearch = false;
        private ObservableCollection<MenuItemDto> _filteredItems = new();

        public ClearancePage(ApiClient api)
        {
            InitializeComponent();
            _api = api;
            ClearanceCollectionView.ItemsSource = _todayClearances;
            MenuItemResultsCollection.ItemsSource = _filteredItems;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadMenuItems();
            await LoadTodayClearances();
        }

        private async Task LoadMenuItems()
        {
            try
            {
                _allMenuItems = await _api.GetMenuItemsAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load menu items: {ex.Message}", "OK");
            }
        }

        private async Task LoadTodayClearances()
        {
            try
            {
                var clearances = await _api.GetTodayClearancesAsync();
                _todayClearances.Clear();
                foreach (var c in clearances)
                    _todayClearances.Add(c);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load clearances: {ex.Message}", "OK");
            }
        }

        private void MenuItemSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppressSearch) return;

            var keyword = e.NewTextValue?.Trim() ?? "";
            _selectedMenuItem = null;

            _filteredItems.Clear();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var filtered = _allMenuItems
                    .Where(i => i.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .Take(20)
                    .ToList();

                foreach (var item in filtered)
                    _filteredItems.Add(item);
            }

            MenuItemResultsCollection.IsVisible = _filteredItems.Count > 0;
        }

        private void MenuItemResultsCollection_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is MenuItemDto selected)
            {
                _selectedMenuItem = selected;
                _suppressSearch = true;
                MenuItemSearchBar.Text = selected.Name;
                _suppressSearch = false;
                MenuItemResultsCollection.IsVisible = false;
                ((ListView)sender).SelectedItem = null;
            }
        }

        private async void SubmitClearance_Clicked(object sender, EventArgs e)
        {
            if (_selectedMenuItem == null)
            {
                await DisplayAlert("Error", "Please select a menu item.", "OK");
                return;
            }

            if (!int.TryParse(QuantityEntry.Text, out int qty) || qty <= 0)
            {
                await DisplayAlert("Error", "Enter a valid quantity.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(ReasonEntry.Text))
            {
                await DisplayAlert("Error", "Reason is required.", "OK");
                return;
            }

            try
            {
                var request = new CreateClearanceRequest
                {
                    MenuItemId = _selectedMenuItem.Id,
                    Quantity = qty,
                    Reason = ReasonEntry.Text.Trim(),
                    Note = string.IsNullOrWhiteSpace(NoteEditor.Text) ? null : NoteEditor.Text.Trim()
                };

                await _api.CreateClearanceAsync(request);
                await DisplayAlert("Success", "Clearance recorded successfully.", "OK");

                await LoadTodayClearances();
                QuantityEntry.Text = string.Empty;
                ReasonEntry.Text = string.Empty;
                NoteEditor.Text = string.Empty;
                _suppressSearch = true;
                MenuItemSearchBar.Text = string.Empty;
                _suppressSearch = false;
                _selectedMenuItem = null;
                _filteredItems.Clear();
                MenuItemResultsCollection.IsVisible = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
