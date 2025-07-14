using APPVitalPets.Models;
using APPVitalPets.Services;
using System;

namespace APPVitalPets.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }


    // Animación cuando aparece la página
    private async void OnPageAppearing(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        TitleLabel.Opacity = 0;
        FormFrame.Opacity = 0;
        TitleLabel.TranslationY = 20;

        await TitleLabel.FadeTo(1, 500, Easing.CubicIn);
        await TitleLabel.TranslateTo(0, 0, 500, Easing.CubicOut);
        await FormFrame.FadeTo(1, 500, Easing.CubicIn);
    }

    // Lógica del botón de inicio de sesión
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var user = UserEntry.Text?.Trim();
        var pass = PassEntry.Text;

        // Validación de campos vacíos
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
        {
            ErrorLabel.Text = "⚠️ Por favor ingresa usuario y contraseña.";
            ErrorLabel.IsVisible = true;
            await ErrorLabel.FadeTo(1, 250);
            return;
        }

        var api = new ApiService();
        var usuario = await api.LoginAsync(user, pass);

        if (usuario != null)
        {
            ErrorLabel.IsVisible = false;
            await Navigation.PushAsync(new MascotasPage(usuario));
        }
        else
        {
            ErrorLabel.Text = "❌ Usuario o contraseña incorrectos.";
            ErrorLabel.IsVisible = true;
            await ErrorLabel.FadeTo(1, 250);
        }

    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistroPage());
    }

}
