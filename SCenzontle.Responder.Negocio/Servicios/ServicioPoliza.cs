using SCenzontle.Responder.Comun.Model;
using SCenzontle.Responder.Persistencia;
using System.Threading.Tasks;

namespace SCenzontle.Responder.Negocio.Servicios
{
    public class ServicioPoliza
    {
        private readonly ApplicationDbContext _dbContext;

        public ServicioPoliza(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> InsertarPoliza(PolizaModel polizaData)
        {
            return await _dbContext.InsertarPolizaAsync(polizaData);
        }
    }
}