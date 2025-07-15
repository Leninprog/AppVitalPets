using APPVitalPets.Models;
using APPVitalPets.Services;
using APPVitalPets.ViewModels;

namespace APPVitalPets.Views;

public partial class RegistroPage : ContentPage
{
    public RegistroPage()
    {
        InitializeComponent();
        BindingContext = new RegistroViewModel();
    }
}
