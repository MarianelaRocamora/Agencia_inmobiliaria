using MySql.Data.MySqlClient;

namespace Agencia_inmobiliaria.Models
{
    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
    {
        public RepositorioUsuario(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Usuario p)
        {
            int id = 0;
            var sql = @"INSERT INTO usuario 
                       (email, password, avatar, nombre, apellido, dni, telefono, direccion, rol, estado) 
                        VALUES (@email, @password, @avatar, @nombre, @apellido, @dni, @telefono, @direccion, @rol, @estado);
                        SELECT LAST_INSERT_ID();";
            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@email", p.Email);
                command.Parameters.AddWithValue("@password", p.Password);
                command.Parameters.AddWithValue("@avatar", (object?)p.Avatar ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombre", p.Nombre);
                command.Parameters.AddWithValue("@apellido", p.Apellido);
                command.Parameters.AddWithValue("@dni", p.Dni);
                command.Parameters.AddWithValue("@telefono", (object?)p.Telefono ?? DBNull.Value);
                command.Parameters.AddWithValue("@direccion", (object?)p.Direccion ?? DBNull.Value);
                command.Parameters.AddWithValue("@rol", p.Rol);
                command.Parameters.AddWithValue("@estado", p.Estado);

                connection.Open();
                id = Convert.ToInt32(command.ExecuteScalar());
            }

            return id;
        }

        public int Baja(int id)
        {
            int filasAfectadas = 0;
            string sql = @"UPDATE usuario SET estado = 0 WHERE ID_usuario = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                filasAfectadas = command.ExecuteNonQuery();
            }

            return filasAfectadas;
        }

        public int Modificacion(Usuario p)
        {
            int filasAfectadas = 0;
            
            var sql = @"UPDATE usuario SET 
                        avatar = @avatar, 
                        nombre=@nombre, 
                        apellido=@apellido, 
                        dni=@dni, 
                        telefono = @telefono, 
                        direccion= @direccion, 
                        rol = @rol, 
                        password = @password 
                        WHERE ID_usuario = @id;";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@avatar", (object?)p.Avatar ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombre", p.Nombre);
                command.Parameters.AddWithValue("@apellido", p.Apellido);
                command.Parameters.AddWithValue("@dni", p.Dni);
                command.Parameters.AddWithValue("@telefono", (object?)p.Telefono ?? DBNull.Value);
                command.Parameters.AddWithValue("@direccion", (object?)p.Direccion ?? DBNull.Value);
                command.Parameters.AddWithValue("@rol", p.Rol);
                command.Parameters.AddWithValue("@password", p.Password);
                command.Parameters.AddWithValue("@id", p.IdUsuario);

                connection.Open();
                filasAfectadas = command.ExecuteNonQuery();
            }
            return filasAfectadas;
        }

        public int ObtenerCantidad()
        {
            int cantidad = 0;
            string sql = @"SELECT COUNT(*) FROM usuario WHERE usuario.estado = 1;";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                connection.Open();
                cantidad = Convert.ToInt32(command.ExecuteScalar());
            }

            return cantidad;
        }

        public IList<Usuario> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var lista = new List<Usuario>();
            string sql = @"SELECT ID_usuario, email, password, avatar, nombre, apellido, dni, telefono, direccion, rol, estado
                            FROM usuario
	                        WHERE estado = 1
	                        ORDER BY ID_Usuario
                            LIMIT @tamPagina OFFSET @offset";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@tamPagina", tamPagina);
                command.Parameters.AddWithValue("@offset", (paginaNro - 1) * tamPagina);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    int avatarOrdinal = reader.GetOrdinal("avatar");
                    int telefonoOrdinal = reader.GetOrdinal("telefono");
                    int direccionOrdinal = reader.GetOrdinal("direccion");
                    int rolOrdinal = reader.GetOrdinal("rol");

                    while (reader.Read())
                    {
                        lista.Add(new Usuario
                        {
                            IdUsuario = reader.GetInt32("ID_usuario"),
                            Email = reader.GetString("email"),
                            Password = reader.GetString("password"),
                            Avatar = reader.IsDBNull(avatarOrdinal) ? null : reader.GetString("avatar"),
                            Nombre = reader.GetString("nombre"),
                            Apellido = reader.GetString("apellido"),
                            Dni = reader.GetString("dni"),
                            Telefono = reader.IsDBNull(telefonoOrdinal) ? null : reader.GetString("telefono"),
                            Direccion = reader.IsDBNull(direccionOrdinal) ? null : reader.GetString("direccion"),
                            Rol = Enum.Parse<RolUsuario>(reader.GetString(rolOrdinal)),
                            Estado = reader.GetBoolean("estado")
                        });
                    }
                }
            }

            return lista;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? usuario = null;
            string sql = @"SELECT ID_usuario, email, password, avatar, nombre, apellido, dni, telefono, direccion, rol, estado
                           FROM usuario
                           WHERE estado = 1
                           AND email = @email";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@email", email);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            IdUsuario = reader.GetInt32("ID_usuario"),
                            Email = reader.GetString("email"),
                            Password = reader.GetString("password"),
                            Avatar = reader.IsDBNull(reader.GetOrdinal("avatar")) ? null : reader.GetString("avatar"),
                            Nombre = reader.GetString("nombre"),
                            Apellido = reader.GetString("apellido"),
                            Dni = reader.GetString("dni"),
                            Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono"),
                            Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString("direccion"),
                            Rol = Enum.Parse<RolUsuario>(reader.GetString(reader.GetOrdinal("rol"))),
                            Estado = reader.GetBoolean("estado")
                        };
                    }
                }
            }

            return usuario;
        }

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? usuario = null;
            string sql = @"SELECT ID_usuario, email, password, avatar, nombre, apellido, dni, telefono, direccion, rol, estado
                           FROM usuario
                           WHERE ID_usuario = @id;";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            IdUsuario = reader.GetInt32("ID_usuario"),
                            Email = reader.GetString("email"),
                            Password = reader.GetString("password"),
                            Avatar = reader.IsDBNull(reader.GetOrdinal("avatar")) ? null : reader.GetString("avatar"),
                            Nombre = reader.GetString("nombre"),
                            Apellido = reader.GetString("apellido"),
                            Dni = reader.GetString("dni"),
                            Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono"),
                            Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString("direccion"),
                            Rol = Enum.Parse<RolUsuario>(reader.GetString(reader.GetOrdinal("rol"))),
                            Estado = reader.GetBoolean("estado")
                        };
                    }
                }
            }

            return usuario;
        }
    }
}