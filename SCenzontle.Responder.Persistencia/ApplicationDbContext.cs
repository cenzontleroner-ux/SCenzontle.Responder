using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SCenzontle.Responder.Comun.Model;


namespace SCenzontle.Responder.Persistencia
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public async Task<List<UserResponse>> GetUsersFromSpAsync(string userEmail)
        {
            var userEmailParam = new SqlParameter("@user_email", userEmail);

            // Llama al stored procedure y mapea los resultados
            return await this.Database
                .SqlQueryRaw<UserResponse>("EXEC [Seguros].[usp_GetUsers] @user_email", userEmailParam)
                .ToListAsync();
        }
        public async Task<List<CatalogoResponse>> CargarCatalogosAsync(string userEmail)
        {
            var userEmailParam = new SqlParameter("@user_email", userEmail);

            // Ejecuta el stored procedure y mapea los resultados
            return await this.Database
                .SqlQueryRaw<CatalogoResponse>("EXEC [Seguros].[usp_CargaCatalogosUnificado] @user_email", userEmailParam)
                .ToListAsync();
        }
        public async Task<int> InsertarPolizaAsync(PolizaModel polizaData)
        {
            var emailParam = new SqlParameter("@emailCliente", polizaData.EmailCliente);
            var numeroPolizaParam = new SqlParameter("@numeroPoliza", polizaData.NumeroPoliza);
            var tipoPolizaParam = new SqlParameter("@idTipoPoliza", polizaData.IdTipoPoliza);
            var statusPolizaParam = new SqlParameter("@idStatusPoliza", polizaData.IdStatusPoliza);
            var fechaInicioParam = new SqlParameter("@fechaInicio", polizaData.FechaInicio);
            var fechaFinParam = new SqlParameter("@fechaFin", polizaData.FechaFin);
            var costoParam = new SqlParameter("@costo", polizaData.Costo);

            // Se usa FromSqlRaw para ejecutar el stored procedure y mapear el resultado
            var result = await this.Database.SqlQueryRaw<NuevaPolizaResponse>(
                "EXEC [Seguros].[usp_InsertPoliza] @emailCliente, @numeroPoliza, @idTipoPoliza, @idStatusPoliza, @fechaInicio, @fechaFin, @costo",
                emailParam,
                numeroPolizaParam,
                tipoPolizaParam,
                statusPolizaParam,
                fechaInicioParam,
                fechaFinParam,
                costoParam
            ).ToListAsync();

            // Retorna el ID de la nueva póliza
            return result.Any() ? result.First().NuevaPolizaId : 0;
        }
    }
}