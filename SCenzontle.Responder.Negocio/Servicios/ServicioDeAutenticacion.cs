using Microsoft.AspNetCore.Identity;
using SCenzontle.Responder.Comun.Model;
using SCenzontle.Responder.Persistencia;
using System.Threading.Tasks;

namespace SCenzontle.Responder.Negocio.Servicios
{
    public class ServicioDeAutenticacion
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public ServicioDeAutenticacion(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<SignInResult> LoginAsync(string email, string password)
        {
            // Busca al usuario por su email
            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario == null)
            {
                // Devolvemos un error genérico para no dar pistas sobre si el usuario existe o no
                return SignInResult.Failed;
            }

            // Intenta autenticar al usuario con la contraseña
            var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, password, lockoutOnFailure: false);

            return resultado;
        }
    }
}