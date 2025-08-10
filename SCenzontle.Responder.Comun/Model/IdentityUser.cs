using Microsoft.AspNetCore.Identity;

namespace SCenzontle.Responder.Comun.Model
{
    public class Usuario : IdentityUser
    {
        public string Nombre { get; set; }
        public string ApellidoMaterno { get; set; }
        public string ApellidoPaterno { get; set; }
        public int Edad { get; set; }
        public string PaisNacimiento { get; set; }
        public string Genero { get; set; }
    }
}