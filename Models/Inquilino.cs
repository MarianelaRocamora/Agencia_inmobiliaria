using System.ComponentModel.DataAnnotations;

namespace Agencia_inmobiliaria.Models
{
   public class Inquilino
    {
        public int IdInquilino { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 30 caracteres")]
        [RegularExpression(@"^[^0-9]+$", ErrorMessage = "El nombre no puede contener números")]
        public required string Nombre { get; set;}

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 30 caracteres")]
        [RegularExpression(@"^[^0-9]+$", ErrorMessage = "El apellido no puede contener números")]
        public required string Apellido { get; set;}

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [RegularExpression(@"^(\d{1,3}\.\d{3}\.\d{3}|\d{7,9})$", ErrorMessage = "Formato de DNI inválido")]
        public required string Dni {get; set;}
       
        [Required(ErrorMessage = "El telefono es obligatorio")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El teléfono debe tener entre 6 y 20 caracteres")]
        [Phone(ErrorMessage ="Formato de telefono no válido")]
        public required string Telefono {get; set;}
        
        public required string Email {get; set;}
        [Required(ErrorMessage = "La direccion es obligatoria")]
        public required string Direccion {get; set;}
        public bool Estado { get; set; } = true;
    } 
}
