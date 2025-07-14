using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPVitalPets.Models
{
    internal class Cita
    {
        public int Id { get; set; }
        public string NombreMascota { get; set; }
        public string NombreDueño { get; set; }
        public string TelefonoContacto { get; set; }
        public DateTime Fecha { get; set; }
        public string Motivo { get; set; }
        public string VeterinarioAsignado { get; set; }
    }
}
