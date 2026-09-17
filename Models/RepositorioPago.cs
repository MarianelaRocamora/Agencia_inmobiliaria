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
            string sql = @"INSERT INTO pago (concepto, fecha_pago, importe, ID_reserva, estado)
                            VALUES (@concepto, @fecha_pago, @importe, @ID_reserva, @estado);
                            SELECT LAST_INSERT_ID();";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@concepto", p.Concepto);
                command.Parameters.AddWithValue("@fecha_pago", p.FechaPago);
                command.Parameters.AddWithValue("@importe", p.Importe);
                command.Parameters.AddWithValue("@ID_reserva", p.IdReserva);
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
            string sql = @"SELECT ID_pago, concepto, fecha_pago, importe, ID_reserva, estado
                            FROM pago
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
                        pago = LeerPago(reader);
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