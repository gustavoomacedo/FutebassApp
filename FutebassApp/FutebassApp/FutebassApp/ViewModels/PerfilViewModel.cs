using FutebassApp.Helpers;
using FutebassApp.Services;
using FutebassApp.Views;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FutebassApp.ViewModels
{
    public class PerfilViewModel : BaseViewModel
    {
        AzureService azureService;
        INavigation navigation;
        public Command _logOutCommand { get; set; }

        private string _foto;
        public string Foto
        {
            get { return _foto; }
            set { SetProperty(ref _foto, value); }
        }

        private string _nome;
        public string Nome
        {
            get { return "Nome: " + _nome; }
            set { SetProperty(ref _nome, value);  }
        }

        private string _nivel;
        public string Nivel
        {
            get { return "Nivel: " + _nivel; }
            set { SetProperty(ref _nivel, value); }
        }

        private string _cidade;
        public string Cidade
        {
            get { return "Cidade: " + _cidade; }
            set { SetProperty(ref _cidade, value); }
        }

        public PerfilViewModel(INavigation nav)
        {
            navigation = nav;
            azureService = DependencyService.Get<AzureService>();
            var perfil = azureService.getJogadorById(Settings.UserId);
            if (perfil != null)
            {
                Foto = perfil.Foto;
                Nome = perfil.Nome;
                Nivel = perfil.Nivel;
                Cidade = perfil.Cidade;
            }
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
                if (page.GetType() != typeof(LoginView))
                    navigation.RemovePage(page);
            }
        }

    }
}
