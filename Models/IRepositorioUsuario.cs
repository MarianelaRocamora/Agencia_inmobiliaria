namespace Agencia_inmobiliaria.Models
{
    public interface IRepositorioUsuario : IRepositorio<Usuario>
    {
        Usuario? ObtenerPorEmail(string email);
    }
}