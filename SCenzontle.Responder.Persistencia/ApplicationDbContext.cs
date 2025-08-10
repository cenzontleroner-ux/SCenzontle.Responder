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
    }
}