using Microsoft.AspNetCore.Identity;
using SCenzontle.Responder.Comun.Model;
using System.Threading.Tasks;


namespace SCenzontle.Responder.Negocio.Servicios
{
    public class ServicioDeRegistro
    {
        private readonly UserManager<Usuario> _userManager;

        public ServicioDeRegistro(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> RegistrarUsuarioAsync(Usuario usuario, string password, string rol)
        {
            var resultado = await _userManager.CreateAsync(usuario, password);

            if (resultado.Succeeded)
            {
                await _userManager.AddToRoleAsync(usuario, rol);
            }

            return resultado;
        }
    }
}
