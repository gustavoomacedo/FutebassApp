using FutebassApp.Models;
using FutebassApp.Services;
using FutebassApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FutebassApp.ViewModels
{
    public class PartidasViewModel : BaseViewModel
    {
        AzureService azureService;
        INavigation navigation;
        public ICommand CarregarPartidas { get; set; }
        public Command AddPartidaCommand { get; }

        public Command HistoricoCommand { get; }

        private ObservableCollection<Partida> _partidas;
        public ObservableCollection<Partida> Partidas
        {
            get { return _partidas; }
            set { SetProperty(ref _partidas, value); }
        }

        public PartidasViewModel(INavigation nav)
        {
            azureService = DependencyService.Get<AzureService>();
            navigation = nav;
            Partidas = new ObservableCollection<Partida>();
            AddPartidaCommand = new Command(ExecuteAddCommand);
            HistoricoCommand = new Command(ExecuteHistoricoCommand);
        }

        private async void ExecuteHistoricoCommand(object obj)
        {
            var perfil = new PerfilView();
            await navigation.PushAsync(perfil);
        }

        private async void ExecuteAddCommand(object obj)
        {
            var add = new AdicaoPartidaView();
            await navigation.PushAsync(add);
        }

        public override async Task LoadAsync()
        {
            IsBusy = true;
            var partidas = await azureService.GetPartidasAsync();

            System.Diagnostics.Debug.WriteLine("FOUND {0} TAGS", partidas.Count);
            Partidas.Clear();
            foreach (var p in partidas)
            {
                Partidas.Add(p);
            }

            OnPropertyChanged(nameof(Partidas));

            IsBusy = false;
        }
    }
}
