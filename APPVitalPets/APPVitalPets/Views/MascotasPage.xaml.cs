using APPVitalPets.Models;
using APPVitalPets.Services;

namespace APPVitalPets.Views;

public partial class MascotasPage : ContentPage
{
    Usuario usuarioActual;
    Mascota mascotaEnEdicion = null;

    public MascotasPage(Usuario user)
    {
        InitializeComponent();
        usuarioActual = user;
        CargarMascotas();
    }

    //Filtramos las mascotas por el ID del usuario, para que se visualice solo las asociadas con el due�o.
    private async void CargarMascotas()
    {
        var api = new ApiService();
        var todas = await api.ObtenerMascotasAsync();
        var mias = todas.Where(m => m.UsuarioId == usuarioActual.Id).ToList(); // Encargada de filtrar
        MascotasList.ItemsSource = mias;
    }

    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
            string.IsNullOrWhiteSpace(RazaEntry.Text) ||
            string.IsNullOrWhiteSpace(EdadEntry.Text))
        {
            await DisplayAlert("Error", "Completa todos los campos", "OK");
            return;
        }

        if (!int.TryParse(EdadEntry.Text, out int edad))
        {
            await DisplayAlert("Error", "Por favor ingresa una edad v�lida (solo n�meros).", "OK");
            return;
        }

        var nueva = new Mascota
        {
            Nombre = NombreEntry.Text,
            Raza = RazaEntry.Text,
            Especie = EspecieEntry.Text,
            Edad = edad,
            UsuarioId = usuarioActual.Id
        };

        var api = new ApiService();
        if (await api.CrearMascotaAsync(nueva))
        {
            await DisplayAlert("�xito", "Mascota creada correctamente", "OK");
            CargarMascotas();

            NombreEntry.Text = "";
            RazaEntry.Text = "";
            EdadEntry.Text = "";
            FormularioMascota.IsVisible = false;
        }
        else
        {
            await DisplayAlert("Error", "No se pudo crear la mascota", "OK");
        }
    }

    private void OnMostrarFormularioClicked(object sender, EventArgs e)
    {
        FormularioMascota.IsVisible = true;
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
        EdadEntry.Text = mascota.Edad.ToString();

        FormularioMascota.IsVisible = true;
        RegistrarBtn.IsVisible = false;
        ActualizarBtn.IsVisible = true;
    }

    private async void OnActualizarClicked(object sender, EventArgs e)
    {
        if (mascotaEnEdicion == null)
            return;

        if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
            string.IsNullOrWhiteSpace(RazaEntry.Text) ||
            string.IsNullOrWhiteSpace(EdadEntry.Text))
        {
            await DisplayAlert("Error", "Completa todos los campos", "OK");
            return;
        }

        if (!int.TryParse(EdadEntry.Text, out int edad))
        {
            await DisplayAlert("Error", "Edad no v�lida", "OK");
            return;
        }

        mascotaEnEdicion.Nombre = NombreEntry.Text;
        mascotaEnEdicion.Raza = RazaEntry.Text;
        mascotaEnEdicion.Especie = EspecieEntry.Text;
        mascotaEnEdicion.Edad = edad;

        var api = new ApiService();
        bool actualizado = await api.ActualizarMascotaAsync(mascotaEnEdicion);

        if (actualizado)
        {
            await DisplayAlert("Actualizado", "Mascota actualizada correctamente", "OK");
            mascotaEnEdicion = null;

            FormularioMascota.IsVisible = false;
            ActualizarBtn.IsVisible = false;
            RegistrarBtn.IsVisible = true;

            NombreEntry.Text = "";
            RazaEntry.Text = "";
            EspecieEntry.Text = "";
            EdadEntry.Text = "";

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

        await DisplayAlert("Detalles", $"Nombre: {mascota.Nombre}\n Raza: {mascota.Raza}\n Especie: {mascota.Especie}\n Edad: {mascota.Edad}", "Cerrar");
    }

    private async void OnEliminarMascota(object sender, EventArgs e)
    {
        var boton = sender as Button;
        var mascota = boton?.CommandParameter as Mascota;
        if (mascota == null) return;

        var confirmacion = await DisplayAlert("Confirmar", $"�Eliminar a {mascota.Nombre}?", "S�", "No");
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

    private void OnMascotaSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedMascota = e.CurrentSelection.FirstOrDefault() as Mascota;
        if (selectedMascota == null)
            return;

        DisplayAlert("Mascota seleccionada", $"Nombre: {selectedMascota.Nombre}", "OK");

        ((CollectionView)sender).SelectedItem = null;
    }
}