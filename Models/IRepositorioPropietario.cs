namespace Agencia_inmobiliaria.Models
{
    public interface IRepositorioPropietario : IRepositorio<Propietario>
	{
		IList<Propietario> Buscar(string texto);
	}
}