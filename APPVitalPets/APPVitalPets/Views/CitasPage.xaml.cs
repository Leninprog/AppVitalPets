using APPVitalPets.Models;
using APPVitalPets.Services;
using APPVitalPets.ViewModels;

namespace APPVitalPets.Views;

public partial class CitasPage : ContentPage
{
    public CitasPage()
    {
        InitializeComponent();
        BindingContext = new CitasViewModel();
    }
}
