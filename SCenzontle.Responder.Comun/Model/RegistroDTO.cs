using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCenzontle.Responder.Comun.Model
{
    public class RegistroDTO
    {
        public string Nombre { get; set; }
        public string ApellidoMaterno { get; set; }
        public string ApellidoPaterno { get; set; }
        public int Edad { get; set; }
        public string PaisNacimiento { get; set; }
        public string Genero { get; set; }
        public string CorreoElectronico { get; set; }
        public string Telefono { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
    }
}
