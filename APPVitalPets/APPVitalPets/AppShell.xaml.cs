using APPVitalPets.Views;

namespace APPVitalPets
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("RegistroPage", typeof(RegistroPage));
        }
    }
}
