using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SCenzontle.Responder.Comun.Model;
using System.Threading.Tasks;
using SCenzontle.Responder.Negocio.Servicios;

[Route("api/[controller]")]
[ApiController]
public class RegistroController : ControllerBase
{
    private readonly ServicioDeRegistro _servicioDeRegistro;

    public RegistroController(ServicioDeRegistro servicioDeRegistro)
    {
        _servicioDeRegistro = servicioDeRegistro;
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistroDTO modelo)
    {
        // Validaciones básicas de modelo
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var usuario = new Usuario
        {
            UserName = modelo.CorreoElectronico,
            Email = modelo.CorreoElectronico,
            PhoneNumber = modelo.Telefono,
            Nombre = modelo.Nombre,
            ApellidoMaterno = modelo.ApellidoMaterno,
            ApellidoPaterno = modelo.ApellidoPaterno,
            Edad = modelo.Edad,
            PaisNacimiento = modelo.PaisNacimiento,
            Genero = modelo.Genero
        };

        var resultado = await _servicioDeRegistro.RegistrarUsuarioAsync(usuario, modelo.Password, modelo.Rol);

        if (resultado.Succeeded)
        {
            return Ok(new { Mensaje = "Usuario registrado exitosamente." });
        }

        return BadRequest(new { Errores = resultado.Errors });
    }
}