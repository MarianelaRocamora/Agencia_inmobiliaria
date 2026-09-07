namespace Agencia_inmobiliaria.Models
{
    public class Imagen{
        public int IdImagen { get; set; }
		public int IdInmueble { get; set; }
		public string Url { get; set; } = "";
        public bool Estado {get; set; } = true;
		public IFormFile? Archivo { get; set; } = null;
    }
        
}