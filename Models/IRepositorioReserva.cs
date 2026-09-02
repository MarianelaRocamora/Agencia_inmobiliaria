namespace Agencia_inmobiliaria.Models
{
    public interface IRepositorioReserva : IRepositorio<Reserva>
	{
		int Cancelar(int id, DateTime fechaCancelacion);
	}
}