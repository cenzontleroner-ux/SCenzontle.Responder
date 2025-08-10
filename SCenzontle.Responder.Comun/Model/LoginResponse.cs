namespace SCenzontle.Responder.Comun.Model
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public int ExpiresIn { get; set; } // En segundos
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? NombreRol { get; set; }
    }
}
