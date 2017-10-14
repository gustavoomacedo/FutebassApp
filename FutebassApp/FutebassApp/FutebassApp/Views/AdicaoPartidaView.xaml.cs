using FutebassApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FutebassApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdicaoPartidaView : ContentPage
    {
        public AdicaoPartidaView()
        {
            InitializeComponent();
            this.BindingContext = new AdicaoPartidaViewModel(Navigation);
        }
    }
}