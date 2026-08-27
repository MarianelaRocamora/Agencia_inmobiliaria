using System.ComponentModel.DataAnnotations;

namespace Agencia_inmobiliaria.Models
{
   public class Inquilino
    {
        public int IdInquilino { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [RegularExpression(@"^[^0-9]+$", ErrorMessage = "El nombre no puede contener números")]
        public required string Nombre { get; set;}
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [RegularExpression(@"^[^0-9]+$", ErrorMessage = "El apellido no puede contener números")]
        public required string Apellido { get; set;}

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [RegularExpression(@"^(\d{1,3}\.\d{3}\.\d{3}|\d{7,9})$", ErrorMessage = "Formato de DNI inválido")]
        public required string Dni {get; set;}

        [Required(ErrorMessage = "El telefono es obligatorio")]
        [Phone(ErrorMessage ="Formato de telefono no válido")]
        public required string Telefono {get; set;}
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email no válido")]
        public required string Email {get; set;}
        [Required(ErrorMessage = "La direccion es obligatoria")]
        public required string Direccion {get; set;}
        public bool Estado { get; set; } = true;
    } 
}
