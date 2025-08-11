using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SCenzontle.Responder.Comun.Model;
using SCenzontle.Responder.Negocio.Servicios;
using System.Threading.Tasks;

namespace SCenzontle.Responder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Protege el endpoint para que solo usuarios autenticados puedan crear pólizas
    public class PolizaController : ControllerBase
    {
        private readonly ServicioPoliza _servicioPoliza;

        public PolizaController(ServicioPoliza servicioPoliza)
        {
            _servicioPoliza = servicioPoliza;
        }

        [HttpPost]
        public async Task<IActionResult> InsertarPoliza([FromBody] PolizaModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var nuevaPolizaId = await _servicioPoliza.InsertarPoliza(modelo);

                if (nuevaPolizaId > 0)
                {
                    return Ok(new { mensaje = "Póliza insertada exitosamente.", nuevaPolizaId = nuevaPolizaId });
                }
                else
                {
                    return BadRequest(new { mensaje = "No se pudo insertar la póliza. Verifique los datos." });
                }
            }
            catch (SqlException ex)
            {
                // Captura el error específico del stored procedure (como el cliente no encontrado)
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}