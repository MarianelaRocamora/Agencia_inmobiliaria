using MySql.Data.MySqlClient;

namespace Agencia_inmobiliaria.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Pago p)
        {
            int id = 0;
            string sql = @"INSERT INTO pago (concepto, fecha_pago, importe, ID_reserva, ID_usuario_creador, estado)
                            VALUES (@concepto, @fecha_pago, @importe, @ID_reserva, @ID_usuario_creador, @estado);
                            SELECT LAST_INSERT_ID();";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@concepto", p.Concepto);
                command.Parameters.AddWithValue("@fecha_pago", p.FechaPago);
                command.Parameters.AddWithValue("@importe", p.Importe);
                command.Parameters.AddWithValue("@ID_reserva", p.IdReserva);
                command.Parameters.AddWithValue("ID_usuario_creador", p.IdUsuarioCreador);
                command.Parameters.AddWithValue("@estado", p.Estado);

                connection.Open();
                id = Convert.ToInt32(command.ExecuteScalar());
            }

            return id;
        }

        public int Baja(int id)
        {
            int filasAfectadas = 0;
            string sql = "UPDATE pago SET estado = 0 WHERE ID_pago = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                filasAfectadas = command.ExecuteNonQuery();
            }

            return filasAfectadas;
        }
        public int Baja(int id, int idUsuarioAnulador)
        {
            int filasAfectadas = 0;
            string sql = "UPDATE pago SET estado = 0, ID_usuario_anulador = @idUsuarioanulador WHERE ID_pago = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@idUsuarioAnulador", idUsuarioAnulador);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                filasAfectadas = command.ExecuteNonQuery();
            }

            return filasAfectadas;
        }

        public int Modificacion(Pago p)
        {
            int filasAfectadas = 0;
            string sql = @"UPDATE pago SET
                            concepto = @concepto
                            WHERE ID_pago = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@concepto", p.Concepto);
                command.Parameters.AddWithValue("@id", p.IdPago);

                connection.Open();
                filasAfectadas = command.ExecuteNonQuery();
            }

            return filasAfectadas;
        }

        public IList<Pago> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var lista = new List<Pago>();
            string sql = @"SELECT ID_pago, concepto, fecha_pago, importe, ID_reserva, estado
                            FROM pago
                            ORDER BY fecha_pago DESC
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
                        lista.Add(LeerPago(reader));
                    }
                }
            }

            return lista;
        }

        public int ObtenerCantidad()
        {
            int cantidad = 0;
            string sql = "SELECT COUNT(*) FROM pago";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                connection.Open();
                cantidad = Convert.ToInt32(command.ExecuteScalar());
            }

            return cantidad;
        }

        public Pago? ObtenerPorId(int id)
        {
            Pago? pago = null;
            string sql = @"SELECT ID_pago, concepto, fecha_pago, importe, ID_reserva, p.estado, ID_usuario_creador, ID_usuario_anulador,
                           uc.nombre AS creador_nombre, uc.apellido AS creador_apellido, uc.email AS creador_email, uc.password AS creador_password, uc.dni AS creador_dni,
                            ua.nombre AS anulador_nombre, ua.apellido AS anulador_apellido, ua.email AS anulador_email, ua.password AS anulador_password, ua.dni AS anulador_dni
                            FROM pago p
                            JOIN usuario uc ON p.ID_usuario_creador = uc.ID_usuario
                            LEFT JOIN usuario ua ON p.ID_usuario_anulador = ua.ID_usuario
                            WHERE ID_pago = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pago = new Pago
                        {
                            IdPago = reader.GetInt32("ID_pago"),
                            Concepto = reader.GetString("concepto"),
                            FechaPago = reader.GetDateTime("fecha_pago"),
                            Importe = reader.GetDecimal("importe"),
                            IdReserva = reader.GetInt32("ID_reserva"),
                            IdUsuarioCreador = reader.GetInt32("ID_usuario_creador"),
                            IdUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("ID_usuario_anulador")) ? null : reader.GetInt32("ID_usuario_anulador"),
                            Estado = reader.GetBoolean("estado"),
                            UsuarioCreador = new Usuario
                            {
                                IdUsuario = reader.GetInt32("ID_usuario_creador"),
                                Nombre = reader.GetString("creador_nombre"),
                                Apellido = reader.GetString("creador_apellido"),
                                Email = reader.GetString("creador_email"),
                                Password = reader.GetString("creador_password"),
                                Dni = reader.GetString("creador_dni")
                            },
                            UsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("ID_usuario_anulador")) ? null : new Usuario
                            {
                                IdUsuario = reader.GetInt32("ID_usuario_anulador"),
                                Nombre = reader.GetString("anulador_nombre"),
                                Apellido = reader.GetString("anulador_apellido"),
                                Email = reader.GetString("anulador_email"),
                                Password = reader.GetString("anulador_password"),
                                Dni = reader.GetString("anulador_dni")
                            }

                        };
                    }
                }
            }

            return pago;
        }

        public IList<Pago> ObtenerPorReserva(int idReserva)
        {
            var lista = new List<Pago>();
            string sql = @"SELECT ID_pago, concepto, fecha_pago, importe, ID_reserva, estado
                            FROM pago
                            WHERE ID_reserva = @idReserva
                            ORDER BY fecha_pago";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@idReserva", idReserva);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(LeerPago(reader));
                    }
                }
            }

            return lista;
        }

        private static Pago LeerPago(MySqlDataReader reader)
        {
            return new Pago
            {
                IdPago = reader.GetInt32("ID_pago"),
                Concepto = reader.GetString("concepto"),
                FechaPago = reader.GetDateTime("fecha_pago"),
                Importe = reader.GetDecimal("importe"),
                IdReserva = reader.GetInt32("ID_reserva"),
                Estado = reader.GetBoolean("estado")
            };
        }
    }
}