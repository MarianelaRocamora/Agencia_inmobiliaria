namespace Agencia_inmobiliaria.Models
{
    public interface IRepositorioImagen: IRepositorio<Imagen>
    {
        IList<Imagen> BuscarPorInmueble(int IdInmueble);
    }
}