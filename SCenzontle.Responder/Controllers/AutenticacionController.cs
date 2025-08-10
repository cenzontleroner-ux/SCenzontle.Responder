using Microsoft.AspNetCore.Mvc;
using SCenzontle.Responder.Comun.Model;
using SCenzontle.Responder.Negocio.Servicios;
using System.Threading.Tasks;

namespace SCenzontle.Responder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly ServicioDeAutenticacion _servicioDeAutenticacion;

        public AutenticacionController(ServicioDeAutenticacion servicioDeAutenticacion)
        {
            _servicioDeAutenticacion = servicioDeAutenticacion;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var respuesta = await _servicioDeAutenticacion.LoginAsync(modelo.Email, modelo.Password);

            if (respuesta != null)
            {
                return Ok(respuesta);
            }

            return Unauthorized(new { mensaje = "Credenciales incorrectas" });
        }
    }
}