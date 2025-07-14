using APPVitalPets.Models;
using APPVitalPets.Services;

namespace APPVitalPets.Views;

public partial class RegistroPage : ContentPage
{
    public RegistroPage()
    {
        InitializeComponent();
    }
    private async void OnVolverMascotasClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MascotasPage");
    }
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var nuevo = new Usuario
        {
            RUsuario = UserEntry.Text,
            Contrasena = PassEntry.Text,
            Correo = CorreoEntry.Text,
            Telefono = TelEntry.Text,
            Direccion = DirEntry.Text
        };

        var api = new ApiService();
        if (await api.RegistrarUsuarioAsync(nuevo))
            await DisplayAlert("OK", "Usuario registrado", "OK");
        else
            await DisplayAlert("Error", "No se pudo registrar", "OK");
    }
}
