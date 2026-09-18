using System.ComponentModel.DataAnnotations;

namespace Agencia_inmobiliaria.Models
{
    public class Pago : IValidatableObject
    {
        public int IdPago { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El concepto debe tener entre 2 y 100 caracteres")]
        public required string Concepto { get; set; }

        [Required(ErrorMessage = "La fecha de pago es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de pago")]
        public DateTime FechaPago { get; set; }

        [Required(ErrorMessage = "El importe es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El importe debe ser mayor a 0")]
        public decimal Importe { get; set; }

        [Required]
        [Display(Name = "Reserva")]
        public int IdReserva { get; set; }

        // true = pago activo, false = anulado (baja lógica, se sigue mostrando)
        public bool Estado { get; set; } = true;

        public Reserva? Reserva { get; set; }

        public int IdUsuarioCreador { get; set; }
        public Usuario? UsuarioCreador { get; set; }

        public int? IdUsuarioAnulador{ get; set; }  
        public Usuario? UsuarioAnulador { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaPago.Date > DateTime.Today)
            {
                yield return new ValidationResult(
                    "La fecha de pago no puede ser futura",
                    [nameof(FechaPago)]);
            }
        }
    }
}