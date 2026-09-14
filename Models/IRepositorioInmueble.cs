namespace  Agencia_inmobiliaria.Models
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        IList<Inmueble> ObtenerDisponiblesEntreFechas(DateTime fechaInicio, DateTime fechaFin, int? idReservaExcluir = null);
        bool InmuebleDisponibleEntreFechas(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idReservaExcluir = null);
        IList<Inmueble> ObtenerPorDisponibilidad(bool? disponible, int paginaNro = 1, int tamPagina = 10);
        int ObtenerCantidadPorDisponibilidad(bool? disponible);
    }
}