  using MySql.Data.MySqlClient;

  namespace Agencia_inmobiliaria.Models
  {
      public class RepositorioTipoInmueble : RepositorioBase, IRepositorioTipoInmueble
      {
          public RepositorioTipoInmueble(IConfiguration configuration) : base(configuration)
          {
          }

          public int Alta(TipoInmueble p)
          {
              int id = 0;
              string sql = @"INSERT INTO tipo_inmueble (nombre, estado)
                              VALUES (@nombre, @estado);
                              SELECT LAST_INSERT_ID();";

              using (var connection = new MySqlConnection(connectionString))
              {
                  var command = new MySqlCommand(sql, connection);
                  command.Parameters.AddWithValue("@nombre", p.Nombre);
                  command.Parameters.AddWithValue("@estado", p.Estado);

                  connection.Open();
                  id = Convert.ToInt32(command.ExecuteScalar());
              }

              return id;
          }

          public int Baja(int id)
          {
              int filasAfectadas = 0;
              string sql = "UPDATE tipo_inmueble SET estado = 0 WHERE ID_tipo_inmueble = @id";

              using (var connection = new MySqlConnection(connectionString))
              {
                  var command = new MySqlCommand(sql, connection);
                  command.Parameters.AddWithValue("@id", id);

                  connection.Open();
                  filasAfectadas = command.ExecuteNonQuery();
              }

              return filasAfectadas;
          }

            public int Modificacion(TipoInmueble p)
            {
                int filasAfectadas = 0;
                string sql = @"UPDATE tipo_inmueble SET
                                nombre = @nombre,
                                estado = @estado
                                WHERE ID_tipo_inmueble = @id";
    
                using (var connection = new MySqlConnection(connectionString))
                {
                    var command = new MySqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@estado", p.Estado);
                    command.Parameters.AddWithValue("@id", p.IdTipoInmueble);
    
                    connection.Open();
                    filasAfectadas = command.ExecuteNonQuery();
                }
    
                return filasAfectadas;
            }

            public IList<TipoInmueble> ObtenerLista(int paginaNro = 1 , int tampagina = 10)
            {
                var lista = new List<TipoInmueble>();
                string sql =
                            @"SELECT ID_tipo_inmueble, nombre, estado
                            FROM tipo_inmueble
                            WHERE estado = 1
                            ORDER BY ID_tipo_inmueble
                            LIMIT @tamPagina OFFSET @offset";

                using (var connection = new MySqlConnection(connectionString))
                {
                    var command = new MySqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@offset", (paginaNro - 1) * tampagina);
                    command.Parameters.AddWithValue("@tampagina", tampagina);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TipoInmueble p = new TipoInmueble
                            {
                                IdTipoInmueble = reader.GetInt32("ID_tipo_inmueble"),
                                Nombre = reader.GetString("nombre"),
                                Estado = reader.GetBoolean("estado")
                            };
                            lista.Add(p);
                        }
                    }
                }

                return lista;
            }

            public int ObtenerCantidad()
            {
                int cantidad = 0;
                string sql = "SELECT COUNT(*) FROM tipo_inmueble WHERE estado = 1";

                using (var connection = new MySqlConnection(connectionString))
                {
                    var command = new MySqlCommand(sql, connection);
                    connection.Open();
                    cantidad = Convert.ToInt32(command.ExecuteScalar());
                }

                return cantidad;
            }

            public TipoInmueble? ObtenerPorId(int id)
            {
                TipoInmueble? tipoInmueble = null;
                string sql = @"SELECT ID_tipo_inmueble, nombre, estado
                            FROM tipo_inmueble
                            WHERE ID_tipo_inmueble = @id";

                using (var connection = new MySqlConnection(connectionString))
                {
                    var command = new MySqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tipoInmueble = new TipoInmueble
                            {
                                IdTipoInmueble = reader.GetInt32("ID_tipo_inmueble"),
                                Nombre = reader.GetString("nombre"),
                                Estado = reader.GetBoolean("estado")
                            };
                        }
                    }
                }

                return tipoInmueble;
            }
      }
  }