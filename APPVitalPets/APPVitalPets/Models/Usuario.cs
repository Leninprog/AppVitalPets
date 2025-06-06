using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPVitalPets.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string RUsuario { get; set; }
        public string Contrasena { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
    }
}
