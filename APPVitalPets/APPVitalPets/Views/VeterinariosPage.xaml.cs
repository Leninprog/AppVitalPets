using APPVitalPets.Models;
using APPVitalPets.Services;
using APPVitalPets.ViewModels;

namespace APPVitalPets.Views;

public partial class VeterinariosPage : ContentPage
{
    public VeterinariosPage()
    {
        InitializeComponent();
        BindingContext = new VeterinariosViewModel();
    }
}
