using System;
using System.Linq;
using APPVitalPets.Models;
using APPVitalPets.Services;
using Microsoft.Maui.Controls;

namespace APPVitalPets.Views
{
    public partial class MascotasPage : ContentPage
    {
        private Usuario usuarioActual;
        private Mascota mascotaEnEdicion = null;

        public MascotasPage(Usuario user)
        {
            InitializeComponent();
            usuarioActual = user;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargarMascotas();
        }

        //Filtramos las mascotas por el ID del usuario, para que se visualice solo las asociadas con el dueño.
        private async void CargarMascotas()
        {
            var api = new ApiService();
            var todas = await api.ObtenerMascotasAsync();
            var mias = todas.Where(m => m.UsuarioId == usuarioActual.Id).ToList(); // Encargada de filtrar
            MascotasList.ItemsSource = mias;
        }

        private async void OnAgregarClicked(object sender, EventArgs e)
        {
            // Validar campos
            if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
                string.IsNullOrWhiteSpace(RazaEntry.Text) ||
                string.IsNullOrWhiteSpace(EspecieEntry.Text))
            {
                await DisplayAlert("Error", "Completa todos los campos", "OK");
                return;
            }

            // Calcular edad a partir de la fecha de nacimiento
            DateTime fecha = FechaNacimientoPicker.Date;
            int edad = DateTime.Today.Year - fecha.Year
                - (fecha > DateTime.Today.AddYears(-(DateTime.Today.Year - fecha.Year)) ? 1 : 0);

            var nueva = new Mascota
            {
                Nombre = NombreEntry.Text,
                Raza = RazaEntry.Text,
                Especie = EspecieEntry.Text,
                FechaNacimiento = fecha,
                Edad = edad,
                UsuarioId = usuarioActual.Id
            };

            var api = new ApiService();
            if (await api.CrearMascotaAsync(nueva))
            {
                await DisplayAlert("Éxito", "Mascota creada correctamente", "OK");
                CargarMascotas();

                // Limpiar formulario
                NombreEntry.Text = string.Empty;
                RazaEntry.Text = string.Empty;
                EspecieEntry.Text = string.Empty;
                FechaNacimientoPicker.Date = DateTime.Today;

                FormularioMascota.IsVisible = false;
                RegistrarBtn.IsVisible = true;
                ActualizarBtn.IsVisible = false;
            }
            else
            {
                await DisplayAlert("Error", "No se pudo crear la mascota", "OK");
            }
        }

        private void OnMostrarFormularioClicked(object sender, EventArgs e)
        {
            // Mostrar formulario en modo “Agregar”
            FormularioMascota.IsVisible = true;
            RegistrarBtn.IsVisible = true;
            ActualizarBtn.IsVisible = false;
        }

        private void OnEditarMascota(object sender, EventArgs e)
        {
            var boton = sender as Button;
            var mascota = boton?.CommandParameter as Mascota;
            if (mascota == null) return;

            mascotaEnEdicion = mascota;
            NombreEntry.Text = mascota.Nombre;
            RazaEntry.Text = mascota.Raza;
            EspecieEntry.Text = mascota.Especie;
            FechaNacimientoPicker.Date = mascota.FechaNacimiento;

            FormularioMascota.IsVisible = true;
            RegistrarBtn.IsVisible = false;
            ActualizarBtn.IsVisible = true;
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

                // Limpiar formulario
                NombreEntry.Text = string.Empty;
                RazaEntry.Text = string.Empty;
                EspecieEntry.Text = string.Empty;
                FechaNacimientoPicker.Date = DateTime.Today;

                FormularioMascota.IsVisible = false;
                RegistrarBtn.IsVisible = true;
                ActualizarBtn.IsVisible = false;

                CargarMascotas();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo actualizar la mascota", "OK");
            }
        }

        private async void OnVerDetalles(object sender, EventArgs e)
        {
            var boton = sender as Button;
            var mascota = boton?.CommandParameter as Mascota;
            if (mascota == null) return;

            await DisplayAlert("Detalles",
                $"Nombre: {mascota.Nombre}\n" +
                $"Raza: {mascota.Raza}\n" +
                $"Especie: {mascota.Especie}\n" +
                $"Edad: {mascota.Edad}\n" +
                $"Fecha Nac: {mascota.FechaNacimiento:dd/MM/yyyy}",
                "Cerrar");
        }

        private async void OnEliminarMascota(object sender, EventArgs e)
        {
            var boton = sender as Button;
            var mascota = boton?.CommandParameter as Mascota;
            if (mascota == null) return;

            bool confirmacion = await DisplayAlert("Confirmar",
                $"¿Eliminar a {mascota.Nombre}?", "Sí", "No");
            if (!confirmacion) return;

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

        private async void OnMascotaSelected(object sender, SelectionChangedEventArgs e)
        {
            var selectedMascota = e.CurrentSelection.FirstOrDefault() as Mascota;
            if (selectedMascota == null)
                return;

            await DisplayAlert(
                "Mascota seleccionada",
                $"Nombre: {selectedMascota.Nombre}",
                "OK"
            );

            ((CollectionView)sender).SelectedItem = null;
        }

    }
}
