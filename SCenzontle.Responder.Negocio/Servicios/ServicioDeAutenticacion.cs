using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SCenzontle.Responder.Comun.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SCenzontle.Responder.Negocio.Servicios
{
    public class ServicioDeAutenticacion
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IConfiguration _configuration;

        public ServicioDeAutenticacion(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario == null)
            {
                return null;
            }

            var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, password, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                var token = await GenerarJwtAsync(usuario, roles);

                // Construye el objeto de respuesta
                return new LoginResponse
                {
                    Token = token,
                    ExpiresIn = 60 * 60 * 24 * 7, // 7 días en segundos
                    Nombre = usuario.Nombre, // O el nombre que tengas en tu clase Usuario
                    Email = usuario.Email,
                    NombreRol = roles.FirstOrDefault() // Asume un solo rol principal
                };
            }

            return null;
        }

        private async Task<string> GenerarJwtAsync(Usuario usuario, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expirationTime = DateTime.Now.AddDays(7);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expirationTime,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}