using FutebassApp.Models;
using FutebassApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FutebassApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PartidasView : ContentPage
    {
        public PartidasView()
        {
            InitializeComponent();
            this.BindingContext = new PartidasViewModel(Navigation);

            //e represents the binded item
            lstPartidas.ItemTapped += async (sender, e) => {
                var partida = e.Item as Partida;
                await App.Current.MainPage.Navigation.PushAsync(new DetalhePartidaView(partida));
            };
        }

        protected override void OnAppearing()
        {
            (BindingContext as PartidasViewModel)?.LoadAsync();
            base.OnAppearing();
        }
    }
}