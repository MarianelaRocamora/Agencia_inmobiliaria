using System.ComponentModel.DataAnnotations;

namespace Agencia_inmobiliaria.Models
{
    public class Reserva: IValidatableObject
    {
        public int IdReserva {get; set;}
        [Required(ErrorMessage ="La fecha de ingreso es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaIngreso {get; set;}
        [Required(ErrorMessage = "La fecha de egreso es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaEgreso {get; set;}
        [DataType(DataType.Date)]
        public DateTime? FechaCancelacion {get; set;}

        [Required(ErrorMessage ="El monto por día es obligatorio")]
        public double MontoDia {get; set;}
        public bool Estado {get; set;} = true;

        public int IdInmueble {get; set;}
        public int IdInquilino {get; set;}

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaEgreso <= FechaIngreso)
            {
                yield return new ValidationResult(
                    "La fecha de egreso debe ser mayor a la fecha de ingreso",
                    [nameof(FechaEgreso)]);
            }
    
        }
    }
}