using System;
using System.Linq;
using APPVitalPets.Models;
using APPVitalPets.Services;
using Microsoft.Maui.Controls;

namespace APPVitalPets.Views
{
    public partial class VeterinariosPage : ContentPage
    {
        private Veterinario vetEnEdicion;

        public VeterinariosPage()
        {
            InitializeComponent();
        }
        private async void OnVolverMascotasClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MascotasPage");
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargarVeterinarios();
        }

        private async void CargarVeterinarios()
        {
            var api = new ApiService();
            VetsList.ItemsSource = await api.ObtenerVeterinariosAsync();
        }

        private void OnMostrarFormularioClicked(object sender, EventArgs e)
        {
            // Mostrar formulario en modo “Agregar”
            FormularioVet.IsVisible    = true;
            RegistrarVetBtn.IsVisible  = true;
            ActualizarVetBtn.IsVisible = false;

            // Limpiar campos
            NombreVetEntry.Text       = string.Empty;
            EspecialidadEntry.Text    = string.Empty;
            TelefonoVetEntry.Text     = string.Empty;
            CorreoVetEntry.Text       = string.Empty;
        }

        private async void OnRegistrarVetClicked(object sender, EventArgs e)
        {
            // Validar campos
            if (string.IsNullOrWhiteSpace(NombreVetEntry.Text) ||
                string.IsNullOrWhiteSpace(EspecialidadEntry.Text) ||
                string.IsNullOrWhiteSpace(TelefonoVetEntry.Text) ||
                string.IsNullOrWhiteSpace(CorreoVetEntry.Text))
            {
                await DisplayAlert("Error", "Completa todos los campos", "OK");
                return;
            }

            var nuevoVet = new Veterinario
            {
                Nombre       = NombreVetEntry.Text,
                Especialidad = EspecialidadEntry.Text,
                Telefono     = TelefonoVetEntry.Text,
                Correo       = CorreoVetEntry.Text
            };

            var api = new ApiService();
            if (await api.CrearVeterinarioAsync(nuevoVet))
            {
                await DisplayAlert("Éxito", "Veterinario registrado correctamente", "OK");
                FormularioVet.IsVisible = false;
                CargarVeterinarios();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo registrar el veterinario", "OK");
            }
        }

        private void OnEditarVetClicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var vet = btn?.CommandParameter as Veterinario;
            if (vet == null) return;

            vetEnEdicion = vet;

            NombreVetEntry.Text       = vet.Nombre;
            EspecialidadEntry.Text    = vet.Especialidad;
            TelefonoVetEntry.Text     = vet.Telefono;
            CorreoVetEntry.Text       = vet.Correo;

            FormularioVet.IsVisible    = true;
            RegistrarVetBtn.IsVisible  = false;
            ActualizarVetBtn.IsVisible = true;
        }

        private async void OnActualizarVetClicked(object sender, EventArgs e)
        {
            if (vetEnEdicion == null) return;

            // Validar campos
            if (string.IsNullOrWhiteSpace(NombreVetEntry.Text) ||
                string.IsNullOrWhiteSpace(EspecialidadEntry.Text) ||
                string.IsNullOrWhiteSpace(TelefonoVetEntry.Text) ||
                string.IsNullOrWhiteSpace(CorreoVetEntry.Text))
            {
                await DisplayAlert("Error", "Completa todos los campos", "OK");
                return;
            }

            // Actualizar datos
            vetEnEdicion.Nombre       = NombreVetEntry.Text;
            vetEnEdicion.Especialidad = EspecialidadEntry.Text;
            vetEnEdicion.Telefono     = TelefonoVetEntry.Text;
            vetEnEdicion.Correo       = CorreoVetEntry.Text;

            var api = new ApiService();
            if (await api.ActualizarVeterinarioAsync(vetEnEdicion))
            {
                await DisplayAlert("Actualizado", "Veterinario actualizado correctamente", "OK");
                FormularioVet.IsVisible = false;
                CargarVeterinarios();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo actualizar el veterinario", "OK");
            }
        }

        private async void OnEliminarVetClicked(object sender, EventArgs e)
        {
            var id = (int)(sender as Button).CommandParameter;
            var api = new ApiService();
            if (await api.EliminarVeterinarioAsync(id))
                await DisplayAlert("Eliminado", "Veterinario eliminado correctamente", "OK");
            CargarVeterinarios();
        }
    }
}
