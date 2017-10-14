using FutebassApp.Models;
using FutebassApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace FutebassApp.ViewModels
{
    public class AdicaoPartidaViewModel : BaseViewModel
    {
        AzureService azureService;
        Geocoder geo;
        public INavigation navegation { get; set; }
        public Command SalvarCommand { get; private set; }

        public AdicaoPartidaViewModel(INavigation _navigation)
        {
            geo = new Geocoder();
            azureService = DependencyService.Get<AzureService>();
            this.navegation = _navigation;
            SalvarCommand = new Command(SalvarPartida, CanExecuteSalvarCommand);
        }

        private bool CanExecuteSalvarCommand()
        {
            return string.IsNullOrWhiteSpace(Dia) == false && string.IsNullOrWhiteSpace(Hora) == false
                 && string.IsNullOrWhiteSpace(Tipo) == false && string.IsNullOrWhiteSpace(Local) == false 
                 && string.IsNullOrWhiteSpace(Observacao) == false;
        }

        private string _dia;
        public string Dia
        {
            get { return _dia; }
            set { SetProperty(ref _dia, value); SalvarCommand.ChangeCanExecute(); }
        }

        private string _hora;
        public string Hora
        {
            get { return _hora; }
            set { SetProperty(ref _hora, value); SalvarCommand.ChangeCanExecute(); }
        }

        private int _preco;
        public int Preco
        {
            get { return _preco; }
            set { SetProperty(ref _preco, value); SalvarCommand.ChangeCanExecute(); }
        }

        private string _tipo;
        public string Tipo
        {
            get { return _tipo; }
            set { SetProperty(ref _tipo, value); SalvarCommand.ChangeCanExecute(); }
        }

        private string _local;
        public string Local
        {
            get { return _local; }
            set { SetProperty(ref _local, value); SalvarCommand.ChangeCanExecute(); }
        }

        private string _observacao;
        public string Observacao
        {
            get { return _observacao; }
            set { SetProperty(ref _observacao, value); SalvarCommand.ChangeCanExecute(); }
        }
        private async void SalvarPartida()
        {
            var partida = new Partida(); 
            partida.Dia = this.Dia;
            partida.Hora = this.Hora;
            partida.Preco = Convert.ToInt32(this.Preco);
            partida.Local = await getCoordenadas(this.Local);
            partida.Observacao = this.Observacao;
            partida.Tipo = this.Tipo;

            var save = await azureService.SalvarPartida(partida);

            if (save)
            {
                await Application.Current.MainPage.DisplayAlert("Sucesso", "Partida adicionada com sucesso!", "OK");
                await navegation.PopAsync();
            }
        }

        private async Task<string> getCoordenadas(string local)
        {
            string retorno = "";

            Position p = new Position();
            var approximateLocations = await geo.GetPositionsForAddressAsync(local);
            foreach (var position in approximateLocations)
                p = position;

            retorno = p.Latitude.ToString() + "," + p.Longitude.ToString();

            return retorno;
        }
    }
}
