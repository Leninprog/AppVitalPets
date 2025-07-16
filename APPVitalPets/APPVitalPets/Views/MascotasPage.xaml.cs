using System;
using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using APPVitalPets.Models;
using APPVitalPets.Services;

namespace APPVitalPets.Views
{
    public partial class MascotasPage : ContentPage
    {
        private Usuario usuarioActual;
        private Mascota mascotaEnEdicion;
        private readonly MascotaDatabase _db;

        public MascotasPage()
        {
            InitializeComponent();
            _db = MascotaDatabase.Instance;

            // 1) Recuperar el usuario guardado en Preferences
            var js = Preferences.Get("usuarioJson", string.Empty);
            if (string.IsNullOrEmpty(js))
            {
                // Si no hay usuario, volver al login
                _ = Shell.Current.GoToAsync("//Login");
                return;
            }

            // 2) Deserializar y cargar mascotas
            usuarioActual = JsonSerializer.Deserialize<Usuario>(js);
            CargarMascotas();
        }

        private async void OnIrCitasClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CitasPage");
        }

        private async void OnIrVeterinariosClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//VeterinariosPage");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargarMascotas();
        }

        private async void CargarMascotas()
        {
            if (usuarioActual == null)
                return;

            var api = new ApiService();
            var todas = await api.ObtenerMascotasAsync();
            var mias = todas.Where(m => m.UsuarioId == usuarioActual.Id).ToList();
            MascotasList.ItemsSource = mias;

            foreach (var mascota in mias)
                await _db.GuardarMascotaAsync(mascota);
        }

        private async void OnAgregarClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
                string.IsNullOrWhiteSpace(RazaEntry.Text) ||
                string.IsNullOrWhiteSpace(EspecieEntry.Text))
            {
                await DisplayAlert("Error", "Completa todos los campos", "OK");
                return;
            }

            DateTime fecha = FechaNacimientoPicker.Date;
            int edad = DateTime.Today.Year - fecha.Year
                - (fecha > DateTime.Today.AddYears(-(DateTime.Today.Year - fecha.Year)) ? 1 : 0);

            var nueva = new Mascota
            {
                Nombre          = NombreEntry.Text,
                Raza            = RazaEntry.Text,
                Especie         = EspecieEntry.Text,
                FechaNacimiento = fecha,
                Edad            = edad,
                UsuarioId       = usuarioActual.Id
            };

            var api = new ApiService();
            if (await api.CrearMascotaAsync(nueva))
            {
                await DisplayAlert("Éxito", "Mascota creada correctamente", "OK");
                CargarMascotas();
                await GuardarUsuarioConMascotasAsync();

                NombreEntry.Text           = string.Empty;
                RazaEntry.Text             = string.Empty;
                EspecieEntry.Text          = string.Empty;
                FechaNacimientoPicker.Date = DateTime.Today;
                FormularioMascota.IsVisible = false;
                RegistrarBtn.IsVisible      = true;
                ActualizarBtn.IsVisible     = false;
            }
            else
            {
                await DisplayAlert("Error", "No se pudo crear la mascota", "OK");
            }
        }

        private void OnMostrarFormularioClicked(object sender, EventArgs e)
        {
            FormularioMascota.IsVisible = true;
            RegistrarBtn.IsVisible      = true;
            ActualizarBtn.IsVisible     = false;
        }

        private void OnEditarMascota(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var mascota = btn?.CommandParameter as Mascota;
            if (mascota == null) return;

            mascotaEnEdicion = mascota;
            NombreEntry.Text           = mascota.Nombre;
            RazaEntry.Text             = mascota.Raza;
            EspecieEntry.Text          = mascota.Especie;
            FechaNacimientoPicker.Date = mascota.FechaNacimiento;
            FormularioMascota.IsVisible = true;
            RegistrarBtn.IsVisible      = false;
            ActualizarBtn.IsVisible     = true;
        }

        private async void OnActualizarClicked(object sender, EventArgs e)
        {
            if (mascotaEnEdicion == null) return;

            if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
                string.IsNullOrWhiteSpace(RazaEntry.Text) ||
                string.IsNullOrWhiteSpace(EspecieEntry.Text))
            {
                await DisplayAlert("Error", "Completa todos los campos", "OK");
                return;
            }

            DateTime fecha = FechaNacimientoPicker.Date;
            int edad = DateTime.Today.Year - fecha.Year
                - (fecha > DateTime.Today.AddYears(-(DateTime.Today.Year - fecha.Year)) ? 1 : 0);

            mascotaEnEdicion.Nombre          = NombreEntry.Text;
            mascotaEnEdicion.Raza            = RazaEntry.Text;
            mascotaEnEdicion.Especie         = EspecieEntry.Text;
            mascotaEnEdicion.FechaNacimiento = fecha;
            mascotaEnEdicion.Edad            = edad;

            var api = new ApiService();
            if (await api.ActualizarMascotaAsync(mascotaEnEdicion))
            {
                await DisplayAlert("Actualizado", "Mascota actualizada correctamente", "OK");
                mascotaEnEdicion = null;
                NombreEntry.Text           = string.Empty;
                RazaEntry.Text             = string.Empty;
                EspecieEntry.Text          = string.Empty;
                FechaNacimientoPicker.Date = DateTime.Today;
                FormularioMascota.IsVisible = false;
                RegistrarBtn.IsVisible      = true;
                ActualizarBtn.IsVisible     = false;
                CargarMascotas();
                await GuardarUsuarioConMascotasAsync();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo actualizar la mascota", "OK");
            }
        }

        private async void OnEliminarMascota(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var mascota = btn?.CommandParameter as Mascota;
            if (mascota == null) return;

            bool ok = await DisplayAlert("Confirmar",
                $"¿Eliminar a {mascota.Nombre}?", "Sí", "No");
            if (!ok) return;

            var api = new ApiService();
            if (await api.EliminarMascotaAsync(mascota.Id))
            {
                await DisplayAlert("Eliminado", $"{mascota.Nombre} fue eliminado correctamente", "OK");
                CargarMascotas();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo eliminar", "OK");
            }
        }

        private async void OnVerDetalles(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var mascota = btn?.CommandParameter as Mascota;
            if (mascota == null) return;

            await DisplayAlert("Detalles",
                $"Nombre: {mascota.Nombre}\n" +
                $"Raza: {mascota.Raza}\n" +
                $"Especie: {mascota.Especie}\n" +
                $"Edad: {mascota.Edad}\n" +
                $"Fecha Nac: {mascota.FechaNacimiento:dd/MM/yyyy}",
                "Cerrar");
        }

        private void OnMascotaSelected(object sender, SelectionChangedEventArgs e)
        {
            var sel = e.CurrentSelection.FirstOrDefault() as Mascota;
            if (sel == null) return;
            DisplayAlert("Mascota seleccionada", $"Nombre: {sel.Nombre}", "OK");
            ((CollectionView)sender).SelectedItem = null;
        }

        private async Task GuardarUsuarioConMascotasAsync()
        {
            var api = new ApiService();
            var todas = await api.ObtenerMascotasAsync();
            var mias = todas.Where(m => m.UsuarioId == usuarioActual.Id).ToList();

            var dataService = new UsuarioDataService();
            await dataService.GuardarUsuarioConMascotasAsync(usuarioActual, mias);
        }

        private async void OnCerrarSesionClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Cerrar Sesión", "¿Estás seguro de que deseas cerrar sesión?", "Sí", "Cancelar");
            if (confirm)
            {
                Preferences.Remove("usuarioJson");

                await Shell.Current.GoToAsync("//Login");
            }
        }

    }
}
