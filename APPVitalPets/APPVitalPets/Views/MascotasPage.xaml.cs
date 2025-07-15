using APPVitalPets.Models;
using APPVitalPets.Services;
using APPVitalPets.ViewModels;

namespace APPVitalPets.Views;

public partial class MascotasPage : ContentPage
{
    public MascotasPage()
    {
        InitializeComponent();
        BindingContext = new MascotasViewModel();
    }
}
