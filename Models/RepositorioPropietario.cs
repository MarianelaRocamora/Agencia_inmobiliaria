using MySql.Data.MySqlClient;

namespace Agencia_inmobiliaria.Models
{
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Propietario p)
        {
            int id = 0;
            string sql = @"INSERT INTO propietario (nombre, apellido, dni, telefono, email, direccion, estado)
                            VALUES (@nombre, @apellido, @dni, @telefono, @email, @direccion, @estado);
                            SELECT LAST_INSERT_ID();";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@nombre", p.Nombre);
                command.Parameters.AddWithValue("@apellido", p.Apellido);
                command.Parameters.AddWithValue("@dni", p.Dni);
                command.Parameters.AddWithValue("@telefono", p.Telefono);
                command.Parameters.AddWithValue("@email", p.Email);
                command.Parameters.AddWithValue("@direccion", p.Direccion);
                command.Parameters.AddWithValue("@estado", p.Estado);

                connection.Open();
                id = Convert.ToInt32(command.ExecuteScalar());
            }

            return id;
        }

        public int Baja(int id)
        {
            int filasAfectadas = 0;
            string sql = "UPDATE propietario SET estado = 0 WHERE ID_propietario = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                filasAfectadas = command.ExecuteNonQuery();
            }

            return filasAfectadas;
        }

        public int Modificacion(Propietario p)
        {
            int filasAfectadas = 0;
            string sql = @"UPDATE propietario SET
                            nombre = @nombre,
                            apellido = @apellido,
                            dni = @dni,
                            telefono = @telefono,
                            email = @email,
                            direccion = @direccion,
                            estado = @estado
                            WHERE ID_propietario = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@nombre", p.Nombre);
                command.Parameters.AddWithValue("@apellido", p.Apellido);
                command.Parameters.AddWithValue("@dni", p.Dni);
                command.Parameters.AddWithValue("@telefono", p.Telefono);
                command.Parameters.AddWithValue("@email", p.Email);
                command.Parameters.AddWithValue("@direccion", p.Direccion);
                command.Parameters.AddWithValue("@estado", p.Estado);
                command.Parameters.AddWithValue("@id", p.IdPropietario);

                connection.Open();
                filasAfectadas = command.ExecuteNonQuery();
            }

            return filasAfectadas;
        }

        public IList<Propietario> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var lista = new List<Propietario>();
            string sql = @"SELECT ID_propietario, nombre, apellido, dni, telefono, email, direccion, estado
                            FROM propietario
                            WHERE estado = 1
                            ORDER BY ID_propietario
                            LIMIT @tamPagina OFFSET @offset";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@tamPagina", tamPagina);
                command.Parameters.AddWithValue("@offset", (paginaNro - 1) * tamPagina);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Propietario
                        {
                            IdPropietario = reader.GetInt32("ID_propietario"),
                            Nombre = reader.GetString("nombre"),
                            Apellido = reader.GetString("apellido"),
                            Dni = reader.GetString("dni"),
                            Telefono = reader.GetString("telefono"),
                            Email = reader.GetString("email"),
                            Direccion = reader.GetString("direccion"),
                            Estado = reader.GetBoolean("estado")
                        });
                    }
                }
            }

            return lista;
        }

        public int ObtenerCantidad()
        {
            int cantidad = 0;
            string sql = "SELECT COUNT(*) FROM propietario WHERE estado = 1";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                connection.Open();
                cantidad = Convert.ToInt32(command.ExecuteScalar());
            }

            return cantidad;
        }

        public Propietario? ObtenerPorId(int id)
        {
            Propietario? propietario = null;
            string sql = @"SELECT ID_propietario, nombre, apellido, dni, telefono, email, direccion, estado
                            FROM propietario
                            WHERE ID_propietario = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        propietario = new Propietario
                        {
                            IdPropietario = reader.GetInt32("ID_propietario"),
                            Nombre = reader.GetString("nombre"),
                            Apellido = reader.GetString("apellido"),
                            Dni = reader.GetString("dni"),
                            Telefono = reader.GetString("telefono"),
                            Email = reader.GetString("email"),
                            Direccion = reader.GetString("direccion"),
                            Estado = reader.GetBoolean("estado")
                        };
                    }
                }
            }

            return propietario;
        }

        public IList<Propietario> Buscar(string texto)
        {
            var lista = new List<Propietario>();
            string sql = @"SELECT ID_propietario, nombre, apellido, dni, telefono, email, direccion, estado
                            FROM propietario
                            WHERE estado = 1
                              AND (nombre LIKE @texto OR apellido LIKE @texto OR dni LIKE @texto)
                            ORDER BY apellido, nombre
                            LIMIT 20";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@texto", "%" + texto + "%");

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Propietario
                        {
                            IdPropietario = reader.GetInt32("ID_propietario"),
                            Nombre = reader.GetString("nombre"),
                            Apellido = reader.GetString("apellido"),
                            Dni = reader.GetString("dni"),
                            Telefono = reader.GetString("telefono"),
                            Email = reader.GetString("email"),
                            Direccion = reader.GetString("direccion"),
                            Estado = reader.GetBoolean("estado")
                        });
                    }
                }
            }

            return lista;
        }
    }
}