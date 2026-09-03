using System.ComponentModel.DataAnnotations;

namespace Agencia_inmobiliaria.Models
{
    public class Inmueble
    {
        public int IdInmueble {get; set;}
        [Required(ErrorMessage = "La direccion es obligatoria")]
        public required string Direccion {get; set;}

        [Required(ErrorMessage =" El cupo es obligatorio")]
        [Range(1, 50, ErrorMessage = "El cupo debe estar entre 1 y 50 personas")]
        [Display(Name = "Cupo (personas)")]
        public  int Cupo {get; set;}

        [Required(ErrorMessage = "El precio por día es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio por día debe ser mayor a 0")]
        [Display(Name = "Precio por día")]
        public decimal PrecioDia {get; set;}


        [Required(ErrorMessage = "El porcentaje de reserva es obligatorio")]
        [Range(0, 100, ErrorMessage = "El porcentaje de reserva debe estar entre 0 y 100")]
        [Display(Name = "Porcentaje de reserva (%)")]
        public decimal PorcentajeReserva {get; set;}


        [Required(ErrorMessage = "Las coordenadas son obligatorias")]
        [Display(Name = "Latitud")]
        public decimal Latitud { get; set; }
        
        [Required(ErrorMessage = "Las coordenadas son obligatorias")]
        [Display(Name = "Longitud")]
		public decimal Longitud { get; set; }

        [Display(Name = "Imagen de Portada")]
        public string? Portada {get; set;}
        
        [Display(Name = "Disponible")]
        public bool Disponible {get; set;} = true;
        

        [Required(ErrorMessage = "El tipo de inmueble es obligatorio")]
        [Display(Name = "Tipo de Inmueble")]
        public int IdTipoInmueble {get; set;}
        [Required(ErrorMessage = "El propietario es obligatorio")]
        [Display(Name = "Propietario")]
        public int IdPropietario {get; set;}
        
        public bool Estado { get; set;} = true;
    }
}