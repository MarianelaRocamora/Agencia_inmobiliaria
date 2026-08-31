using System.ComponentModel.DataAnnotations;

namespace Agencia_inmobiliaria.Models
{
    public class TipoInmueble
    {
        public int IdTipoInmueble {set; get;}

        [Required(ErrorMessage = "El campo es obligatorio")]
        public required string Nombre {set; get;}

        public bool Estado {set; get;} = true;
    }
}