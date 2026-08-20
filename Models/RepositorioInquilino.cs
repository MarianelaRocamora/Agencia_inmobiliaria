using MySql.Data.MySqlClient;

namespace Agencia_inmobiliaria.Models
{
    public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Inquilino p)
        {
            int id = 0;
            string sql = @"INSERT INTO inquilino (nombre, apellido, dni, telefono, email, direccion, estado)
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
            string sql = "UPDATE inquilino SET estado = 0 WHERE ID_inquilino = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                filasAfectadas = command.ExecuteNonQuery();
            }

            return filasAfectadas;
        }

        public int Modificacion(Inquilino p)
        {
            int filasAfectadas = 0;
            string sql = @"UPDATE inquilino SET
                            nombre = @nombre,
                            apellido = @apellido,
                            dni = @dni,
                            telefono = @telefono,
                            email = @email,
                            direccion = @direccion,
                            estado = @estado
                            WHERE ID_inquilino = @id";

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
                command.Parameters.AddWithValue("@id", p.IdInquilino);

                connection.Open();
                filasAfectadas = command.ExecuteNonQuery();
            }

            return filasAfectadas;
        }

        public IList<Inquilino> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var lista = new List<Inquilino>();
            string sql = @"SELECT ID_inquilino, nombre, apellido, dni, telefono, email, direccion, estado
                            FROM inquilino
                            WHERE estado = 1
                            ORDER BY ID_inquilino
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
                        lista.Add(new Inquilino
                        {
                            IdInquilino = reader.GetInt32("ID_inquilino"),
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
            string sql = "SELECT COUNT(*) FROM inquilino WHERE estado = 1";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                connection.Open();
                cantidad = Convert.ToInt32(command.ExecuteScalar());
            }

            return cantidad;
        }

        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? inquilino = null;
            string sql = @"SELECT ID_inquilino, nombre, apellido, dni, telefono, email, direccion, estado
                            FROM inquilino
                            WHERE ID_inquilino = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        inquilino = new Inquilino
                        {
                            IdInquilino = reader.GetInt32("ID_inquilino"),
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

            return inquilino;
        }
    }
}