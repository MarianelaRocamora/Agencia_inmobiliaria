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
            string sql = @"INSERT INTO reserva (fecha_ingreso, fecha_egreso, monto_dia, ID_inmueble, ID_inquilino, ID_usuario_creador, estado)
                            VALUES (@fecha_ingreso, @fecha_egreso, @monto_dia, @ID_inmueble, @ID_inquilino, @ID_usuario_creador, @estado);
                            SELECT LAST_INSERT_ID();";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@fecha_ingreso", p.FechaIngreso);
                command.Parameters.AddWithValue("@fecha_egreso", p.FechaEgreso);
                command.Parameters.AddWithValue("@monto_dia", p.MontoDia);
                command.Parameters.AddWithValue("@ID_inmueble", p.IdInmueble);
                command.Parameters.AddWithValue("@ID_inquilino", p.IdInquilino);
                command.Parameters.AddWithValue("@ID_Usuario_Creador", p.IdUsuarioCreador);
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

        public int Cancelar(int id, DateTime fechaCancelacion, int idUsuarioFinalizador)
        {
            int filasAfectadas = 0;
            string sql = @"UPDATE reserva SET
                            fecha_cancelacion = @fecha_cancelacion, ID_usuario_finalizador = @idUsuarioFinalizador
                            WHERE ID_reserva = @id AND fecha_cancelacion IS NULL";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@fecha_cancelacion", fechaCancelacion);
                command.Parameters.AddWithValue("@idUsuarioFinalizador", idUsuarioFinalizador);
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
                            WHERE ID_reserva = @id";

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
            string sql = @"SELECT r.ID_reserva, r.fecha_ingreso, r.fecha_egreso, r.monto_dia, r.ID_inmueble, r.ID_inquilino, r.fecha_cancelacion, r.estado,
                                  i.nombre AS inq_nombre, i.apellido AS inq_apellido, i.dni AS inq_dni, i.telefono, i.email, i.direccion AS inq_direccion,
                                  inm.direccion AS inm_direccion, inm.precio_dia AS inm_precioDia
                            FROM reserva r
                            JOIN inquilino i ON r.ID_inquilino = i.ID_inquilino
                            JOIN inmueble inm ON r.ID_inmueble = inm.ID_inmueble
                            WHERE r.estado = 1
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
                            MontoDia = reader.GetDecimal("monto_dia"),
                            IdInmueble = reader.GetInt32("ID_inmueble"),
                            IdInquilino = reader.GetInt32("ID_inquilino"),
                            FechaCancelacion = reader.IsDBNull(reader.GetOrdinal("fecha_cancelacion"))
                                               ? (DateTime?)null
                                               : reader.GetDateTime("fecha_cancelacion"),
                            Estado = reader.GetBoolean("estado"),
                             Inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32("ID_inquilino"),
                                Nombre = reader.GetString("inq_nombre"),
                                Apellido = reader.GetString("inq_apellido"),
                                Dni = reader.GetString("inq_dni"),
                                Telefono = reader.GetString("telefono"),
                                Email = reader.GetString("email"),
                                Direccion = reader.GetString("inq_direccion")
                            },
                            Inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("ID_inmueble"),
                                Direccion = reader.GetString("inm_direccion"),
                                PrecioDia = reader.GetDecimal("inm_precioDia")
                            }
                        });
                    }
                }
            }

            return lista;
        }

        public Reserva? ObtenerPorId(int id)
        {
            Reserva? reserva = null;
            string sql = @"SELECT r.ID_reserva, r.fecha_ingreso, r.fecha_egreso, r.monto_dia, r.ID_inmueble, r.ID_inquilino, r.fecha_cancelacion, r.estado, r.ID_usuario_creador, r.ID_usuario_finalizador,
                                  i.nombre AS inq_nombre, i.apellido AS inq_apellido, i.dni AS inq_dni, i.telefono, i.email, i.direccion AS inq_direccion,
                                  inm.direccion AS inm_direccion, inm.precio_dia AS inm_precioDia, inm.cupo AS inm_cupo, inm.ID_tipo_inmueble AS inm_idTipoInmueble, inm.porcentaje_reserva,
                                  ti.nombre AS tipoNombre,
                                  uc.nombre AS creador_nombre, uc.apellido AS creador_apellido, uc.email AS creador_email, uc.password AS creador_password, uc.dni AS creador_dni,
                                  uf.nombre AS finalizador_nombre, uf.apellido AS finalizador_apellido, uf.email AS finalizador_email, uf.password AS finalizador_password, uf.dni AS finalizador_dni
                            FROM reserva r
                            JOIN inquilino i ON r.ID_inquilino = i.ID_inquilino
                            JOIN inmueble inm ON r.ID_inmueble = inm.ID_inmueble
                            JOIN tipo_inmueble ti ON inm.ID_tipo_inmueble = ti.ID_tipo_inmueble
                            JOIN usuario uc ON r.ID_usuario_creador = uc.ID_usuario
                            LEFT JOIN usuario uf ON r.ID_usuario_finalizador = uf.ID_usuario
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
                            MontoDia = reader.GetDecimal("monto_dia"),
                            IdInmueble = reader.GetInt32("ID_inmueble"),
                            IdInquilino = reader.GetInt32("ID_inquilino"),
                            FechaCancelacion = reader.IsDBNull(reader.GetOrdinal("fecha_cancelacion"))
                                               ? (DateTime?)null
                                               : reader.GetDateTime("fecha_cancelacion"),
                            Estado = reader.GetBoolean("estado"),
                            IdUsuarioCreador = reader.GetInt32("ID_usuario_creador"),
                            IdUsuarioFinalizador = reader.IsDBNull(reader.GetOrdinal("ID_usuario_finalizador")) ? null : reader.GetInt32("ID_usuario_finalizador"),
                            Inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32("ID_inquilino"),
                                Nombre = reader.GetString("inq_nombre"),
                                Apellido = reader.GetString("inq_apellido"),
                                Dni = reader.GetString("inq_dni"),
                                Telefono = reader.GetString("telefono"),
                                Email = reader.GetString("email"),
                                Direccion = reader.GetString("inq_direccion")
                            },
                            Inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("ID_inmueble"),
                                Direccion = reader.GetString("inm_direccion"),
                                PrecioDia = reader.GetDecimal("inm_precioDia"),
                                Cupo = reader.GetInt32("inm_cupo"),
                                PorcentajeReserva = reader.GetDecimal("porcentaje_reserva"),
                                IdTipoInmueble = reader.GetInt32("inm_idTipoInmueble"),
                                TipoInmueble = new TipoInmueble
                                {
                                    IdTipoInmueble = reader.GetInt32("inm_idTipoInmueble"),
                                    Nombre = reader.GetString("tipoNombre")
                                }
                            },
                             UsuarioCreador = new Usuario
                            {
                                IdUsuario = reader.GetInt32("ID_usuario_creador"),
                                Nombre = reader.GetString("creador_nombre"),
                                Apellido = reader.GetString("creador_apellido"),
                                Email = reader.GetString("creador_email"),
                                Password = reader.GetString("creador_password"),
                                Dni = reader.GetString("creador_dni")
                            },

                            UsuarioFinalizador = reader.IsDBNull(reader.GetOrdinal("ID_usuario_finalizador")) ? null : new Usuario
                            {
                                IdUsuario = reader.GetInt32("ID_usuario_finalizador"),
                                Nombre = reader.GetString("finalizador_nombre"),
                                Apellido = reader.GetString("finalizador_apellido"),
                                Email = reader.GetString("finalizador_email"),
                                Password = reader.GetString("finalizador_password"),
                                Dni = reader.GetString("finalizador_dni")
                            }

                        };
                    }
                }
            }

            return reserva;
        }

        public IList<Reserva> ObtenerVigentes(int paginaNro = 1, int tamPagina = 10)
        {
            if (paginaNro < 1) paginaNro = 1;
            if (tamPagina < 1) tamPagina = 10;

            var lista = new List<Reserva>();

            string sql = @"SELECT
                                r.ID_reserva, r.fecha_ingreso, r.fecha_egreso, r.monto_dia, r.ID_inmueble, r.ID_inquilino, r.fecha_cancelacion, r.estado,
                                i.nombre AS inq_nombre,
                                i.apellido AS inq_apellido,
                                i.dni AS inq_dni,
                                i.telefono,
                                i.email,
                                i.direccion AS inq_direccion,
                                inm.direccion AS inm_direccion,
                                inm.precio_dia AS inm_precioDia
                            FROM
                                reserva r
                            JOIN inquilino i ON
                                r.ID_inquilino = i.ID_inquilino
                            JOIN inmueble inm ON
                                r.ID_inmueble = inm.ID_inmueble
                            WHERE
                                r.estado = 1
                                AND r.fecha_ingreso <= CURDATE()
                                AND COALESCE(r.fecha_cancelacion, r.fecha_egreso) >= CURDATE()
                            ORDER BY
                                ID_reserva
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
                            MontoDia = reader.GetDecimal("monto_dia"),
                            IdInmueble = reader.GetInt32("ID_inmueble"),
                            IdInquilino = reader.GetInt32("ID_inquilino"),
                            FechaCancelacion = reader.IsDBNull(reader.GetOrdinal("fecha_cancelacion"))
                                               ? (DateTime?)null
                                               : reader.GetDateTime("fecha_cancelacion"),
                            Estado = reader.GetBoolean("estado"),
                             Inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32("ID_inquilino"),
                                Nombre = reader.GetString("inq_nombre"),
                                Apellido = reader.GetString("inq_apellido"),
                                Dni = reader.GetString("inq_dni"),
                                Telefono = reader.GetString("telefono"),
                                Email = reader.GetString("email"),
                                Direccion = reader.GetString("inq_direccion")
                            },
                            Inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("ID_inmueble"),
                                Direccion = reader.GetString("inm_direccion"),
                                PrecioDia = reader.GetDecimal("inm_precioDia"),
                                PorcentajeReserva = reader.GetDecimal("inm_porcentajeReserva"),
                                Cupo = reader.GetInt32("inm_cupo"),
                                IdTipoInmueble = reader.GetInt32("inm_idTipoInmueble"),
                                TipoInmueble = new TipoInmueble
                                {
                                    IdTipoInmueble = reader.GetInt32("inm_idTipoInmueble"),
                                    Nombre = reader.GetString("tipoNombre")
                                }
                            }
                        });
                    }
                }
            }

            return lista;

        }
        public int ObtenerCantidadVigentes()
        {
            int cantidad = 0;
            string sql = @"SELECT COUNT(*) FROM reserva 
                        WHERE estado = 1
                            AND fecha_ingreso <= CURDATE()
                            AND COALESCE(fecha_cancelacion, fecha_egreso) >= CURDATE()";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                connection.Open();
                cantidad = Convert.ToInt32(command.ExecuteScalar());
            }

            return cantidad;
        }

        public IList<Reserva> ObtenerQueTerminanEn(int cantidadDias, int paginaNro = 1, int tamPagina = 10)
        {
            var lista = new List<Reserva>();
            string sql = @"SELECT 
                            r.ID_reserva, r.fecha_ingreso, r.fecha_egreso, r.monto_dia, r.ID_inmueble, r.ID_inquilino, r.fecha_cancelacion, r.estado,
                                i.nombre AS inq_nombre,
                                i.apellido AS inq_apellido,
                                i.dni AS inq_dni,
                                i.telefono,
                                i.email,
                                i.direccion AS inq_direccion,
                                inm.direccion AS inm_direccion,
                                inm.precio_dia AS inm_precioDia
                            FROM
                                reserva r
                            JOIN inquilino i ON
                                r.ID_inquilino = i.ID_inquilino
                            JOIN inmueble inm ON
                                r.ID_inmueble = inm.ID_inmueble
                           WHERE r.estado = 1
                             AND COALESCE(r.fecha_cancelacion, r.fecha_egreso) BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL @cantidadDias DAY)
                            ORDER BY COALESCE(r.fecha_cancelacion, r.fecha_egreso)
                           LIMIT @tamPagina OFFSET @offset";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@cantidadDias", cantidadDias);
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
                            MontoDia = reader.GetDecimal("monto_dia"),
                            IdInmueble = reader.GetInt32("ID_inmueble"),
                            IdInquilino = reader.GetInt32("ID_inquilino"),
                            FechaCancelacion = reader.IsDBNull(reader.GetOrdinal("fecha_cancelacion"))
                                               ? (DateTime?)null
                                               : reader.GetDateTime("fecha_cancelacion"),
                            Estado = reader.GetBoolean("estado"),
                             Inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32("ID_inquilino"),
                                Nombre = reader.GetString("inq_nombre"),
                                Apellido = reader.GetString("inq_apellido"),
                                Dni = reader.GetString("inq_dni"),
                                Telefono = reader.GetString("telefono"),
                                Email = reader.GetString("email"),
                                Direccion = reader.GetString("inq_direccion")
                            },
                            Inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("ID_inmueble"),
                                Direccion = reader.GetString("inm_direccion"),
                                PrecioDia = reader.GetDecimal("inm_precioDia")
                            }
                        });
                    }
                }
            }

            return lista;
        }

        public int ObtenerCantidadQueTerminanEn(int cantidadDias)
        {
            int cantidad = 0;
            string sql = @"SELECT COUNT(*) FROM reserva 
                           WHERE estado = 1
                           AND COALESCE(fecha_cancelacion, fecha_egreso) BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL @cantidadDias DAY)";

            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@cantidadDias", cantidadDias);

                connection.Open();
                cantidad = Convert.ToInt32(command.ExecuteScalar());
            }

            return cantidad;
        }
    }
}