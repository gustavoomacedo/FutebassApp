using System;
using FutebassApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;

namespace FutebassApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetalhePartidaView : ContentPage
    {
        private Partida partida;
        Geocoder geo;

        public DetalhePartidaView()
        {
            InitializeComponent();
        }

        public DetalhePartidaView(Partida _partida)
        {
            this.geo = new Geocoder();
            this.partida = _partida;
            Title = "Detalhe da partida";

            var coordenadas = _partida.Local.Split(',');
            string _lt = coordenadas[0].Replace(".", ",");
            string _lg = coordenadas[1].Replace(".", ",");
            double lt = Convert.ToDouble(_lt);
            double lg = Convert.ToDouble(_lg);

            var position = new Position(lt, lg); // Latitude, Longitude

            var retorno = getEnderecoByCoordenadas(position);

            var map = new Map(MapSpan.FromCenterAndRadius(position, Distance.FromMiles(0.3)))
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            var pin = new Pin
            {
                Type = PinType.Generic,
                Position = position,
                Label = "Arena Neymar",
                Address = "Society"
            };

            map.Pins.Add(pin);

            Label lblNome = new Label { Text = "Criador: " + partida.Jogador.Nome, Margin = 5, HorizontalTextAlignment = TextAlignment.Center };
            Label lblDia = new Label { Text = "Dia: " + partida.Dia, Margin = 5, HorizontalTextAlignment = TextAlignment.Center };
            Label lblHora = new Label { Text = "Hora: " + partida.Hora, Margin = 5, HorizontalTextAlignment = TextAlignment.Center };
            Label lblPreco = new Label { Text = "Valor: R$" + partida.Preco, Margin = 5, HorizontalTextAlignment = TextAlignment.Center };
            Label lblTipo = new Label { Text = "Modalidade: " + partida.Tipo, Margin = 5, HorizontalTextAlignment = TextAlignment.Center };
            Label lblObservacao = new Label { Text = "Observações: " + partida.Observacao, Margin = 5, HorizontalTextAlignment = TextAlignment.Center };
            
            var stack = new StackLayout { Spacing = 30 };
            stack.Children.Add(lblNome);
            stack.Children.Add(lblDia);
            stack.Children.Add(lblHora);
            stack.Children.Add(lblPreco);
            stack.Children.Add(lblTipo);
            stack.Children.Add(lblObservacao);
            stack.Children.Add(map);
            Content = stack;

        }

        public async Task<string> getEnderecoByCoordenadas(Position p)
        {
            string end = "";

            var possibleAddresses = await geo.GetAddressesForPositionAsync(p);
            foreach (var address in possibleAddresses)
                end = address;

            return end;
        }
    }
}