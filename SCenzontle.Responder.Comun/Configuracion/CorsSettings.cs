using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCenzontle.Responder.Comun.Configuracion
{
    public class CorsSettings
    {
        public string PolicyName { get; set; }
        public string[] AllowedOrigins { get; set; }
        public string[] AllowedHeaders { get; set; }
        public string[] AllowedMethods { get; set; }
    }
}
