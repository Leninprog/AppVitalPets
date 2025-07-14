using System;
using System.Linq;
using APPVitalPets.Models;
using APPVitalPets.Services;
using Microsoft.Maui.Controls;

namespace APPVitalPets.Views
{
    public partial class CitasPage : ContentPage
    {
        private Cita citaEnEdicion;

        private async void OnVolverMascotasClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MascotasPage");
        }
        public CitasPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargarCitas();
        }

        private async void CargarCitas()
        {
            var api = new ApiService();
            var lista = await api.ObtenerCitasAsync();
            CitasList.ItemsSource = lista;
        }

        private void OnMostrarFormularioClicked(object sender, EventArgs e)
        {
            FormularioCita.IsVisible    = true;
            RegistrarCitaBtn.IsVisible   = true;
            ActualizarCitaBtn.IsVisible  = false;
            MascotaEntry.Text   = string.Empty;
            DuenoEntry.Text     = string.Empty;
            TelefonoEntry.Text  = string.Empty;
            FechaCitaPicker.Date= DateTime.Today;
            MotivoEntry.Text    = string.Empty;
            VetEntry.Text       = string.Empty;
        }

        private async void OnRegistrarCitaClicked(object sender, EventArgs e)
        {
            // Validar campos requeridos
            if (string.IsNullOrWhiteSpace(MascotaEntry.Text) ||
                string.IsNullOrWhiteSpace(DuenoEntry.Text)   ||
                string.IsNullOrWhiteSpace(TelefonoEntry.Text)||
                string.IsNullOrWhiteSpace(MotivoEntry.Text)  ||
                string.IsNullOrWhiteSpace(VetEntry.Text))
            {
                await DisplayAlert("Error", "Completa todos los campos", "OK");
                return;
            }

            var nueva = new Cita
            {
                NombreMascota       = MascotaEntry.Text,
                NombreDueño         = DuenoEntry.Text,
                TelefonoContacto    = TelefonoEntry.Text,
                Fecha               = FechaCitaPicker.Date,
                Motivo              = MotivoEntry.Text,
                VeterinarioAsignado = VetEntry.Text
            };

            var api = new ApiService();
            if (await api.CrearCitaAsync(nueva))
            {
                await DisplayAlert("Éxito", "Cita registrada correctamente", "OK");
                FormularioCita.IsVisible = false;
                CargarCitas();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo registrar la cita", "OK");
            }
        }

        private void OnEditarCitaClicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var cita = btn?.CommandParameter as Cita;
            if (cita == null) return;

            citaEnEdicion = cita;
            MascotaEntry.Text        = cita.NombreMascota;
            DuenoEntry.Text          = cita.NombreDueño;
            TelefonoEntry.Text       = cita.TelefonoContacto;
            FechaCitaPicker.Date     = cita.Fecha;
            MotivoEntry.Text         = cita.Motivo;
            VetEntry.Text            = cita.VeterinarioAsignado;

            FormularioCita.IsVisible   = true;
            RegistrarCitaBtn.IsVisible  = false;
            ActualizarCitaBtn.IsVisible = true;
        }

        private async void OnActualizarCitaClicked(object sender, EventArgs e)
        {
            if (citaEnEdicion == null) return;

            if (string.IsNullOrWhiteSpace(MascotaEntry.Text) ||
                string.IsNullOrWhiteSpace(DuenoEntry.Text)   ||
                string.IsNullOrWhiteSpace(TelefonoEntry.Text)||
                string.IsNullOrWhiteSpace(MotivoEntry.Text)  ||
                string.IsNullOrWhiteSpace(VetEntry.Text))
            {
                await DisplayAlert("Error", "Completa todos los campos", "OK");
                return;
            }

            citaEnEdicion.NombreMascota       = MascotaEntry.Text;
            citaEnEdicion.NombreDueño         = DuenoEntry.Text;
            citaEnEdicion.TelefonoContacto    = TelefonoEntry.Text;
            citaEnEdicion.Fecha               = FechaCitaPicker.Date;
            citaEnEdicion.Motivo              = MotivoEntry.Text;
            citaEnEdicion.VeterinarioAsignado = VetEntry.Text;

            var api = new ApiService();
            if (await api.ActualizarCitaAsync(citaEnEdicion))
            {
                await DisplayAlert("Actualizado", "Cita actualizada correctamente", "OK");
                FormularioCita.IsVisible = false;
                CargarCitas();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo actualizar la cita", "OK");
            }
        }

        private async void OnEliminarCitaClicked(object sender, EventArgs e)
        {
            var id = (int)(sender as Button).CommandParameter;
            var api = new ApiService();
            if (await api.EliminarCitaAsync(id))
                await DisplayAlert("Eliminado", "Cita eliminada", "Cerrar");
            CargarCitas();
        }
    }
}
