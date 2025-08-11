using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCenzontle.Responder.Comun.Model;
using SCenzontle.Responder.Negocio.Servicios;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SCenzontle.Responder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requiere un token JWT válido
    public class CatalogoController : ControllerBase
    {
        private readonly ServicioCatalogo _servicioCatalogo;

        public CatalogoController(ServicioCatalogo servicioCatalogo)
        {
            _servicioCatalogo = servicioCatalogo;
        }

        [HttpGet]
        public async Task<IActionResult> GetCatalogos()
        {
            // Extrae el email del token JWT
            var userEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { mensaje = "Token inválido o email no disponible." });
            }

            var catalogos = await _servicioCatalogo.ObtenerCatalogos(userEmail);

            // Manejo del mensaje de error del stored procedure
            if (catalogos.Count == 1 && catalogos.First().Desc == "Error: El usuario no existe o no tiene un rol asignado.")
            {
                return BadRequest(new { mensaje = "Error de permisos: El usuario no tiene rol." });
            }

            // Si el stored procedure no devolvió ningún error, procesamos la respuesta
            // Agrupar los resultados por tipoCatalogo para una respuesta más estructurada
            var respuestaAgrupada = catalogos.GroupBy(c => c.TipoCatalogo)
                                             .ToDictionary(g => g.Key, g => g.ToList());

            return Ok(respuestaAgrupada);
        }
    }
}