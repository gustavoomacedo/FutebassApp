using System;
using System.Threading.Tasks;
using FutebassApp.Helpers;
using Xamarin.Forms;
using System.Linq;
using FutebassApp.Views;
using FutebassApp.Services;

namespace FutebassApp.ViewModels
{
    public class MainViewModel
    {
        public string UserId { get; private set; }
        public string Token { get; private set; }

        INavigation navigation;
        AzureService azureService;

        public Command _logOutCommand { get; set; }

        public MainViewModel(INavigation nav)
        {
            azureService = DependencyService.Get<AzureService>();
            navigation = nav;
            UserId = Settings.UserId;
            Token = Settings.AuthToken;
        }

        public Command LogOutCommand =>
            _logOutCommand ?? (_logOutCommand = new Command(async () => await ExecuteLogoutCommandAsync()));

        private async Task ExecuteLogoutCommandAsync()
        {
            await azureService.Logout();
            var login = new LoginView();
            await navigation.PushAsync(login);

            RemovePageFromSatck();
        }

        private void RemovePageFromSatck()
        {
            var existingPages = navigation.NavigationStack.ToList();
            foreach (var page in existingPages)
            {
                if (page.GetType() == typeof(MainPage))
                    navigation.RemovePage(page);
            }
        }
    }
}
