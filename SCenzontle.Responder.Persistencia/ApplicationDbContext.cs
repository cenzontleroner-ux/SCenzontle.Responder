using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SCenzontle.Responder.Comun.Model;
using System.Data;
using Microsoft.Data.SqlClient;


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
            // Crie o parâmetro de saída para capturar o ID
            var nuevaPolizaIdParam = new SqlParameter
            {
                ParameterName = "@nuevaPolizaId",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            var emailParam = new SqlParameter("@emailCliente", polizaData.EmailCliente);
            var numeroPolizaParam = new SqlParameter("@numeroPoliza", polizaData.NumeroPoliza);
            var tipoPolizaParam = new SqlParameter("@idTipoPoliza", polizaData.IdTipoPoliza);
            var statusPolizaParam = new SqlParameter("@idStatusPoliza", polizaData.IdStatusPoliza);
            var fechaInicioParam = new SqlParameter("@fechaInicio", polizaData.FechaInicio);
            var fechaFinParam = new SqlParameter("@fechaFin", polizaData.FechaFin);
            var costoParam = new SqlParameter("@costo", polizaData.Costo);

            // Use ExecuteSqlRawAsync para executar o stored procedure
            // Ele não espera um conjunto de resultados, mas pode lidar com parâmetros de saída
            await this.Database.ExecuteSqlRawAsync(
                "EXEC [Seguros].[usp_InsertPoliza] @emailCliente, @numeroPoliza, @idTipoPoliza, @idStatusPoliza, @fechaInicio, @fechaFin, @costo, @nuevaPolizaId OUTPUT",
                emailParam,
                numeroPolizaParam,
                tipoPolizaParam,
                statusPolizaParam,
                fechaInicioParam,
                fechaFinParam,
                costoParam,
                nuevaPolizaIdParam // Adicione o parâmetro de saída na chamada
            );

            // Recupere o valor do parâmetro de saída
            return (int)nuevaPolizaIdParam.Value;
        }
    }
}