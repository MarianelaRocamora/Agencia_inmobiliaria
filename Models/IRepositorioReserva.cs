namespace Agencia_inmobiliaria.Models
{
    public interface IRepositorioReserva : IRepositorio<Reserva>
	{
		int Cancelar(int id, DateTime fechaCancelacion, int idUsuarioFinalizador);
		IList<Reserva> ObtenerVigentes(int paginaNro = 1, int tamPagina = 10);
		int ObtenerCantidadVigentes();
		IList<Reserva> ObtenerQueTerminanEn(int cantidadDias, int paginaNro = 1, int tamPagina = 10);
		int ObtenerCantidadQueTerminanEn(int cantidadDias);

	}
}