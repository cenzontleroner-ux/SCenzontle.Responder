using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SCenzontle.Responder.Comun.Model;
using SCenzontle.Responder.Negocio.Servicios;
using System.Threading.Tasks;

namespace SCenzontle.Responder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class UsuarioController : ControllerBase
    {
        private readonly ServicioDeAutenticacion _servicioDeAutenticacion;

        public UsuarioController(ServicioDeAutenticacion servicioDeAutenticacion)
        {
            _servicioDeAutenticacion = servicioDeAutenticacion;
        }

        [HttpPost("cambiar-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool sessionActiva = User?.Identity?.IsAuthenticated??false;
            if(!sessionActiva)
            {
                return Unauthorized(new { mensaje = "Sessión expiró." });
            }
            // Aquí se extrae el email del token JWT, no se confía en el email del cuerpo de la petición
            var emailDesdeToken = User?.Identity?.Name??null;

            if (emailDesdeToken == null || emailDesdeToken != modelo.Email)
            {
                return Unauthorized(new { mensaje = "Acceso no autorizado para este email." });
            }

            var resultado = await _servicioDeAutenticacion.CambiarContrasenaAsync(
                modelo.Email!, // El modelo ya está validado
                modelo.ContrasenaActual!,
                modelo.NuevaContrasena!
            );

            if (resultado.Succeeded)
            {
                return Ok(new { mensaje = "Contraseña cambiada exitosamente." });
            }

            return BadRequest(new { mensaje = resultado.Errors.FirstOrDefault()?.Description });
        }
    }
}