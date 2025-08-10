using SCenzontle.Responder.Comun.Model;
using SCenzontle.Responder.Persistencia;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCenzontle.Responder.Negocio.Servicios
{
    public class ServicioUsuario
    {
        private readonly ApplicationDbContext _dbContext;

        public ServicioUsuario(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserResponse>> ObtenerUsuarios(string email)
        {
            return await _dbContext.GetUsersFromSpAsync(email);
        }
    }
}