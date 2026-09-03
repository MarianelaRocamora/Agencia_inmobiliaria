using MySql.Data.MySqlClient;

namespace Agencia_inmobiliaria.Models
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Inmueble entidad)
        {
            int id = 0;
            string sql = @"INSERT INTO inmueble
                            (direccion, cupo, precio_dia, porcentaje_reserva, latitud, longitud, portada, disponible, ID_tipo_inmueble, ID_propietario, estado)
                            VALUES
                            (@direccion, @cupo, @precioDia, @porcentajeReserva, @latitud, @longitud, @portada, @disponible, @idTipoInmueble, @idPropietario, @estado);
                            SELECT LAST_INSERT_ID();";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@direccion", entidad.Direccion);
                command.Parameters.AddWithValue("@cupo", entidad.Cupo);
                command.Parameters.AddWithValue("@precioDia", entidad.PrecioDia);
                command.Parameters.AddWithValue("@porcentajeReserva", entidad.PorcentajeReserva);
                command.Parameters.AddWithValue("@latitud", entidad.Latitud);
                command.Parameters.AddWithValue("@longitud", entidad.Longitud);
                command.Parameters.AddWithValue("@portada", (object?)entidad.Portada ?? DBNull.Value);
                command.Parameters.AddWithValue("@disponible", entidad.Disponible);
                command.Parameters.AddWithValue("@idTipoInmueble", entidad.IdTipoInmueble);
                command.Parameters.AddWithValue("@idPropietario", entidad.IdPropietario);
                command.Parameters.AddWithValue("@estado", entidad.Estado);

                connection.Open();
                id = Convert.ToInt32(command.ExecuteScalar());
            }

            return id;
        }

        public int Baja(int id)
        {
            int filasAfectadas = 0;
            string sql = "UPDATE inmueble SET estado = 0 WHERE ID_inmueble = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                filasAfectadas = command.ExecuteNonQuery();
            }

            return filasAfectadas;
        }

        public int Modificacion(Inmueble entidad)
        {
            int filasAfectadas = 0;
            string sql = @"UPDATE inmueble SET
                            direccion = @direccion,
                            cupo = @cupo,
                            precio_dia = @precioDia,
                            porcentaje_reserva = @porcentajeReserva,
                            latitud = @latitud,
                            longitud = @longitud,
                            portada = @portada,
                            disponible = @disponible,
                            ID_tipo_inmueble = @idTipoInmueble,
                            ID_propietario = @idPropietario,
                            estado = @estado
                            WHERE ID_inmueble = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@direccion", entidad.Direccion);
                command.Parameters.AddWithValue("@cupo", entidad.Cupo);
                command.Parameters.AddWithValue("@precioDia", entidad.PrecioDia);
                command.Parameters.AddWithValue("@porcentajeReserva", entidad.PorcentajeReserva);
                command.Parameters.AddWithValue("@latitud", entidad.Latitud);
                command.Parameters.AddWithValue("@longitud", entidad.Longitud);
                command.Parameters.AddWithValue("@portada", (object?)entidad.Portada ?? DBNull.Value);
                command.Parameters.AddWithValue("@disponible", entidad.Disponible);
                command.Parameters.AddWithValue("@idTipoInmueble", entidad.IdTipoInmueble);
                command.Parameters.AddWithValue("@idPropietario", entidad.IdPropietario);
                command.Parameters.AddWithValue("@estado", entidad.Estado);
                command.Parameters.AddWithValue("@id", entidad.IdInmueble);

                connection.Open();
                filasAfectadas = command.ExecuteNonQuery();
            }

            return filasAfectadas;
        }

        public IList<Inmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var lista = new List<Inmueble>();
            string sql = @"SELECT ID_inmueble, direccion, cupo, precio_dia, porcentaje_reserva, latitud, longitud, portada, disponible, ID_tipo_inmueble, ID_propietario, estado
                            FROM inmueble
                            WHERE estado = 1
                            ORDER BY ID_inmueble
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
                        lista.Add(new Inmueble
                        {
                            IdInmueble = reader.GetInt32("ID_inmueble"),
                            Direccion = reader.GetString("direccion"),
                            Cupo = reader.GetInt32("cupo"),
                            PrecioDia = reader.GetDecimal("precio_dia"),
                            PorcentajeReserva = reader.GetDecimal("porcentaje_reserva"),
                            Latitud = reader.GetDecimal("latitud"),
                            Longitud = reader.GetDecimal("longitud"),
                            Portada = reader.IsDBNull(reader.GetOrdinal("portada")) ? null : reader.GetString("portada"),
                            Disponible = reader.GetBoolean("disponible"),
                            IdTipoInmueble = reader.GetInt32("ID_tipo_inmueble"),
                            IdPropietario = reader.GetInt32("ID_propietario"),
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
            string sql = "SELECT COUNT(*) FROM inmueble WHERE estado = 1";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                connection.Open();
                cantidad = Convert.ToInt32(command.ExecuteScalar());
            }

            return cantidad;
        }

        public Inmueble? ObtenerPorId(int id)
        {
            Inmueble? inmueble = null;
            string sql = @"SELECT ID_inmueble, direccion, cupo, precio_dia, porcentaje_reserva, latitud, longitud, portada, disponible, ID_tipo_inmueble, ID_propietario, estado
                            FROM inmueble
                            WHERE ID_inmueble = @id";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32("ID_inmueble"),
                            Direccion = reader.GetString("direccion"),
                            Cupo = reader.GetInt32("cupo"),
                            PrecioDia = reader.GetDecimal("precio_dia"),
                            PorcentajeReserva = reader.GetDecimal("porcentaje_reserva"),
                            Latitud = reader.GetDecimal("latitud"),
                            Longitud = reader.GetDecimal("longitud"),
                            Portada = reader.IsDBNull(reader.GetOrdinal("portada")) ? null : reader.GetString("portada"),
                            Disponible = reader.GetBoolean("disponible"),
                            IdTipoInmueble = reader.GetInt32("ID_tipo_inmueble"),
                            IdPropietario = reader.GetInt32("ID_propietario"),
                            Estado = reader.GetBoolean("estado")
                        };
                    }
                }
            }

            return inmueble;
        }

        public IList<Inmueble> ObtenerDisponiblesEntreFechas(DateTime fechaInicio, DateTime fechaFin, int? idReservaExcluir = null)
        {
            var lista = new List<Inmueble>();
            string sql = @"SELECT inm.ID_inmueble, inm.direccion, inm.cupo, inm.ID_tipo_inmueble, 
                            inm.latitud, inm.longitud, inm.precio_dia, inm.porcentaje_reserva, 
                            inm.ID_propietario, inm.estado, inm.disponible, inm.portada,
                            ti.nombre AS tipoNombre
                           FROM inmueble inm
                           JOIN tipo_inmueble ti ON inm.ID_tipo_inmueble = ti.ID_tipo_inmueble
                           WHERE inm.estado = 1
                             AND inm.disponible = 1
                             AND NOT EXISTS (
                                 SELECT 1 FROM reserva r
                                 WHERE r.ID_inmueble = inm.ID_inmueble
                                   AND r.estado = 1
                                   AND r.fecha_cancelacion IS NULL
                                   AND r.fecha_ingreso < @fechaIngreso
                                   AND r.fecha_egreso > @fechaEgreso
                                   AND (@idReservaExcluir IS NULL OR r.ID_reserva <> @idReservaExcluir)
                             )
                           ORDER BY inm.direccion";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@fechaIngreso", fechaInicio);
                command.Parameters.AddWithValue("@fechaEgreso", fechaFin);
                command.Parameters.AddWithValue("@idReservaExcluir", (object?)idReservaExcluir ?? DBNull.Value);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Inmueble
                        {
                            IdInmueble = reader.GetInt32("ID_inmueble"),
                            Direccion = reader.GetString("direccion"),
                            Cupo = reader.GetInt32("cupo"),
                            IdTipoInmueble = reader.GetInt32("ID_tipo_inmueble"),
                            Latitud = reader.GetDecimal("latitud"),
                            Longitud = reader.GetDecimal("longitud"),
                            PrecioDia = reader.GetDecimal("precio_dia"),
                            PorcentajeReserva = reader.GetDecimal("porcentaje_reserva"),
                            IdPropietario = reader.GetInt32("ID_propietario"),
                            Estado = reader.GetBoolean("estado"),
                            Disponible = reader.GetBoolean("disponible"),
                            Portada = reader.IsDBNull(reader.GetOrdinal("portada")) ? null : reader.GetString("portada"),
                            TipoInmueble = new TipoInmueble
                            {
                                IdTipoInmueble = reader.GetInt32("ID_tipo_inmueble"),
                                Nombre = reader.GetString("tipoNombre")
                            }
                        });
                    }
                }
            }

            return lista;
        }

        public bool InmuebleDisponibleEntreFechas(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idReservaExcluir = null)
        {
            string sql = @"SELECT COUNT(*) FROM reserva r
                           WHERE r.ID_inmueble = @idInmueble
                             AND r.estado = 1
                             AND r.fecha_cancelacion IS NULL
                             AND r.fecha_ingreso < @fechaFin
                             AND r.fecha_egreso > @fechaInicio
                             AND (@idReservaExcluir IS NULL OR r.ID_reserva <> @idReservaExcluir)";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@idInmueble", idInmueble);
                command.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                command.Parameters.AddWithValue("@fechaFin", fechaFin);
                command.Parameters.AddWithValue("@idReservaExcluir", (object?)idReservaExcluir ?? DBNull.Value);

                connection.Open();
                int cantidadSolapadas = Convert.ToInt32(command.ExecuteScalar());

                return cantidadSolapadas == 0;
            }
        }
    }
}