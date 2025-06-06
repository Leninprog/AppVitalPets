using APPVitalPets.Models;
using APPVitalPets.Services;

namespace APPVitalPets.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var user = UserEntry.Text;
        var pass = PassEntry.Text;
        var api = new ApiService();
        var usuario = await api.LoginAsync(user, pass);
        if (usuario != null)
            await Navigation.PushAsync(new MascotasPage(usuario));
        else
            await DisplayAlert("Error", "Usuario no encontrado", "OK");
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistroPage());
    }
}
