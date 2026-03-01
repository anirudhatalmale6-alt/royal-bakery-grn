using RoyalBakeryGrn.Pages;
using RoyalBakeryGrn.Services;

namespace RoyalBakeryGrn
{
    public partial class App : Application
    {
        private readonly ApiClient _api;

        public App(ApiClient api)
        {
            InitializeComponent();
            _api = api;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Start with login page
            return new Window(new NavigationPage(new LoginPage(_api))
            {
                BarBackgroundColor = Color.FromArgb("#1A1A1A"),
                BarTextColor = Colors.White
            });
        }
    }
}
