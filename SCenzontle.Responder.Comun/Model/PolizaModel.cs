using System;
using System.ComponentModel.DataAnnotations;

namespace SCenzontle.Responder.Comun.Model
{
    public class PolizaModel
    {
        [Required(ErrorMessage = "El email del cliente es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string? EmailCliente { get; set; }

        [Required(ErrorMessage = "El número de póliza es obligatorio.")]
        public string? NumeroPoliza { get; set; }

        [Required(ErrorMessage = "El ID del tipo de póliza es obligatorio.")]
        public int IdTipoPoliza { get; set; }

        [Required(ErrorMessage = "El ID del status de póliza es obligatorio.")]
        public int IdStatusPoliza { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "El costo es obligatorio.")]
        public decimal Costo { get; set; }
    }
}