using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SCenzontle.Responder.Comun.Model;
using SCenzontle.Responder.Negocio.Servicios;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SCenzontle.Responder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class UsuarioController : ControllerBase
    {
        private readonly ServicioDeAutenticacion _servicioDeAutenticacion;
        private readonly ServicioUsuario _servicioUsuario;

        // Constructor corregido para inyectar ambos servicios
        public UsuarioController(ServicioDeAutenticacion servicioDeAutenticacion, ServicioUsuario servicioUsuario)
        {
            _servicioDeAutenticacion = servicioDeAutenticacion;
            _servicioUsuario = servicioUsuario;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            // Extrae el email del token JWT (ClaimTypes.Email o JwtRegisteredClaimNames.Sub)
            var userEmail = User.Identity.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                // Si el token no tiene el claim de email, es un token inválido para este endpoint
                return Unauthorized(new { mensaje = "Token inválido: no se pudo obtener el email del usuario." });
            }

            var usuarios = await _servicioUsuario.ObtenerUsuarios(userEmail);

            // Manejo de errores devueltos por el stored procedure
            if (usuarios.Count == 1 && usuarios.First().Nombre == null)
            {
                return BadRequest(new { mensaje = "Error: El usuario no existe o no tiene un rol asignado." });
            }

            return Ok(usuarios);
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

            var userGRoup = User.Claims.Select(p => new { p.Type, p.Value }).Where(q=>q.Type.Contains("claims/role"));

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