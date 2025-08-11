using SCenzontle.Responder.Comun.Model;
using SCenzontle.Responder.Persistencia;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCenzontle.Responder.Negocio.Servicios
{
    public class ServicioCatalogo
    {
        private readonly ApplicationDbContext _dbContext;

        public ServicioCatalogo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CatalogoResponse>> ObtenerCatalogos(string email)
        {
            return await _dbContext.CargarCatalogosAsync(email);
        }
    }
}