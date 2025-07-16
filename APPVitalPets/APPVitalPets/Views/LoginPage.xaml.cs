using APPVitalPets.Models;
using APPVitalPets.Services;
using System;
using System.Text.Json;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;

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
            // Guarda el usuario en Preferences como JSON
            var js = JsonSerializer.Serialize(usuario);
            Preferences.Set("usuarioJson", js);

            // Guardar archivo local
            var apiService = new ApiService();
            var mascotasDelUsuario = await apiService.ObtenerMascotasAsync();
            var propias = mascotasDelUsuario.Where(m => m.UsuarioId == usuario.Id).ToList();
            var dataService = new UsuarioDataService();
            await dataService.GuardarUsuarioConMascotasAsync(usuario, propias);

            ErrorLabel.IsVisible = false;
            // Navega al tab de Mascotas
            await Shell.Current.GoToAsync("//MascotasPage");
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
