using MySql.Data.MySqlClient;

namespace Agencia_inmobiliaria.Models
{
    public class RepositorioReserva : RepositorioBase, IRepositorioReserva
    {
        public RepositorioReserva(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Reserva p)
        {
            int id = 0;
            string sql = @"INSERT INTO reserva (fecha_ingreso, fecha_egreso, monto_dia, ID_inmueble, ID_inquilino, estado)
                            VALUES (@fecha_ingreso, @fecha_egreso, @monto_dia, @ID_inmueble, @ID_inquilino, @estado);
                            SELECT LAST_INSERT_ID();";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@fecha_ingreso", p.FechaIngreso);
                command.Parameters.AddWithValue("@fecha_egreso", p.FechaEgreso);
                command.Parameters.AddWithValue("@monto_dia", p.MontoDia);
                command.Parameters.AddWithValue("@ID_inmueble", p.IdInmueble);
                command.Parameters.AddWithValue("@ID_inquilino", p.IdInquilino);
                command.Parameters.AddWithValue("@estado", p.Estado);

                connection.Open();
                id = Convert.ToInt32(command.ExecuteScalar());
            }

            return id;
        }

        public int Baja(int id)
        {
            int filasAfectadas = 0;
            string sql = "UPDATE reserva SET estado = 0 WHERE ID_reserva = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                filasAfectadas = command.ExecuteNonQuery();
            }

            return filasAfectadas;
        }

        public int Cancelar(int id, DateTime fechaCancelacion)
        {
            int filasAfectadas = 0;
            string sql = @"UPDATE reserva SET
                            fecha_cancelacion = @fecha_cancelacion
                            WHERE ID_reserva = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@fecha_cancelacion", fechaCancelacion);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                filasAfectadas = command.ExecuteNonQuery();
            }

                return filasAfectadas;
        }

        public int Modificacion(Reserva p)
        {
            int filasAfectadas = 0;
            string sql = @"UPDATE reserva SET
                            fecha_ingreso = @fecha_ingreso,
                            fecha_egreso = @fecha_egreso,
                            monto_dia = @monto_dia,
                            ID_inmueble = @ID_inmueble,
                            ID_inquilino = @ID_inquilino,
                            estado = @estado
                            WHERE ID_inquilino = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@fecha_ingreso", p.FechaIngreso);
                command.Parameters.AddWithValue("@fecha_egreso", p.FechaEgreso);
                command.Parameters.AddWithValue("@monto_dia", p.MontoDia);
                command.Parameters.AddWithValue("@ID_inmueble", p.IdInmueble);
                command.Parameters.AddWithValue("@ID_inquilino", p.IdInquilino);
                command.Parameters.AddWithValue("@estado", p.Estado);
                command.Parameters.AddWithValue("@id", p.IdReserva);

                connection.Open();
                filasAfectadas = command.ExecuteNonQuery();
            }

            return filasAfectadas;
        }

        public int ObtenerCantidad()
        {
            int cantidad = 0;
            string sql = "SELECT COUNT(*) FROM reserva WHERE estado = 1";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                connection.Open();
                cantidad = Convert.ToInt32(command.ExecuteScalar());
            }

            return cantidad;
        }

        public IList<Reserva> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            if (paginaNro < 1) paginaNro = 1;
            if (tamPagina < 1) tamPagina = 10;
            var lista = new List<Reserva>();
            string sql = @"SELECT ID_reserva, fecha_ingreso, fecha_egreso, monto_dia, ID_inmueble, ID_inquilino, fecha_cancelacion, estado
                            FROM reserva
                            WHERE estado = 1
                            ORDER BY ID_reserva
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
                        lista.Add(new Reserva
                        {
                            IdReserva = reader.GetInt32("ID_reserva"),
                            FechaIngreso = reader.GetDateTime("fecha_ingreso"),
                            FechaEgreso = reader.GetDateTime("fecha_egreso"),
                            MontoDia = reader.GetDouble("monto_dia"),
                            IdInmueble = reader.GetInt32("ID_inmueble"),
                            IdInquilino = reader.GetInt32("ID_inquilino"),
                            FechaCancelacion = reader.IsDBNull(reader.GetOrdinal("fecha_cancelacion"))
                                               ? (DateTime?)null
                                               : reader.GetDateTime("fecha_cancelacion"),
                            Estado = reader.GetBoolean("estado")
                        });
                    }
                }
            }

            return lista;
        }

        public Reserva? ObtenerPorId(int id)
        {
            Reserva? reserva = null;
            string sql = @"SELECT ID_reserva, fecha_ingreso, fecha_egreso, monto_dia, ID_inmueble, ID_inquilino, fecha_cancelacion, estado
                            FROM reserva
                            WHERE ID_reserva = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        reserva = new Reserva
                        {
                            IdReserva = reader.GetInt32("ID_reserva"),
                            FechaIngreso = reader.GetDateTime("fecha_ingreso"),
                            FechaEgreso = reader.GetDateTime("fecha_egreso"),
                            MontoDia = reader.GetDouble("monto_dia"),
                            IdInmueble = reader.GetInt32("ID_inmueble"),
                            IdInquilino = reader.GetInt32("ID_inquilino"),
                            FechaCancelacion = reader.IsDBNull(reader.GetOrdinal("fecha_cancelacion"))
                                               ? (DateTime?)null
                                               : reader.GetDateTime("fecha_cancelacion"),
                            Estado = reader.GetBoolean("estado")
                        };
                    }
                }
            }

            return reserva;
        }
    }
}