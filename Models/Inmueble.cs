using System.ComponentModel.DataAnnotations;

namespace Agencia_inmobiliaria.Models
{
    public class Inmueble
    {
        public int IdInmueble {get; set;}
        [Required(ErrorMessage = "La direccion es obligatoria")]
        public required string Direccion {get; set;}

        [Required(ErrorMessage =" El cupo es obligatorio")]
        public  int Cupo {get; set;}
        [Required(ErrorMessage = "El precio por día es obligatorio")]
        public decimal PrecioDia {get; set;}
        [Required(ErrorMessage = "El porcentaje de reserva es obligatorio")]
        public decimal PorcentajeReserva {get; set;}
        [Required(ErrorMessage = "Las coordenadas son obligatorias")]
        public decimal Latitud { get; set; }
        
        [Required(ErrorMessage = "Las coordenadas son obligatorias")]

		public decimal Longitud { get; set; }

        public string? Portada {get; set;}
        public bool Disponible {get; set;} = true;
        [Required(ErrorMessage = "El tipo de inmueble es obligatorio")]
        public int IdTipoInmueble {get; set;}
        [Required(ErrorMessage = "El propietario es obligatorio")]
        public int IdPropietario {get; set;}
        
        public bool Estado { get; set;} = true;
    }
}