using FutebassApp.Helpers;
using FutebassApp.Services;
using FutebassApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FutebassApp.ViewModels
{
    public class LoginViewModel
    {
        public string Title { get; private set; }
        private bool IsBusy;

        AzureService azureService;
        INavigation navigation;

        ICommand loginCommand;
        public ICommand LoginCommand =>
            loginCommand ?? (loginCommand = new Command(async () => await ExecuteLoginCommandAsync()));

        public LoginViewModel(INavigation nav)
        {
            azureService = DependencyService.Get<AzureService>();
            navigation = nav;

            Title = "FutebassApp";
        }

        private async Task ExecuteLoginCommandAsync()
        {
            if (IsBusy || !(await LoginAsync()))
            {
                return;
            }
            else
            {
                var partidas = new PartidasView();
                await navigation.PushAsync(partidas);

                RemovePageFromSatck();
            }
            IsBusy = false;
        }

        private void RemovePageFromSatck()
        {
            var existingPages = navigation.NavigationStack.ToList();
            foreach (var page in existingPages)
            {
                if (page.GetType() == typeof(LoginView))
                    navigation.RemovePage(page);
            }
        }

        public Task<bool> LoginAsync()
        {
            //IsBusy = true;
            if (Settings.IsLoggedIn)
            {
                return Task.FromResult(true);
            }

            return azureService.LoginAsync();
        }
    }
}
