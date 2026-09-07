using System.Data.SqlTypes;
using MySql.Data.MySqlClient;

namespace Agencia_inmobiliaria.Models
{
    public class RepositorioImagen : RepositorioBase, IRepositorioImagen
    {
        public RepositorioImagen(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Imagen p)
        {
            int id = 0;
            string sql = @"INSERT INTO imagen (url, ID_inmueble, estado)
                           VALUES(@url, @IdInmueble, estado);
                           SELECT LAST_INSERT_ID();";
            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@url", p.Url);
                command.Parameters.AddWithValue("@IdInmueble", p.IdInmueble);
                command.Parameters.AddWithValue("@estado", p.Estado);

                connection.Open();
                  id = Convert.ToInt32(command.ExecuteScalar());
            }
            return id;
        }

        public int Baja(int id)
        {
            int filasAfectadas = 0;
              string sql = "UPDATE imagen SET estado = 0 WHERE ID_imagen = @id";

              using (var connection = new MySqlConnection(connectionString))
              {
                  var command = new MySqlCommand(sql, connection);
                  command.Parameters.AddWithValue("@id", id);

                  connection.Open();
                  filasAfectadas = command.ExecuteNonQuery();
              }

              return filasAfectadas;
        }

        public IList<Imagen> BuscarPorInmueble(int IdInmueble)
        {
            var lista = new List<Imagen>();
            var sql = @"SELECT ID_imagen, url, ID_inmueble, estado
                            FROM imagen
                            WHERE estado = 1 AND ID_inmueble = @IdInmueble
                            ORDER BY ID_imagen;";
             using (var connection = new MySqlConnection(connectionString))
                {
                    var command = new MySqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@IdInmueble", IdInmueble);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Imagen p = new Imagen
                            {
                                IdImagen = reader.GetInt32("ID_imagen"),
                                Url = reader.GetString("url"),
                                IdInmueble = reader.GetInt32("ID_inmueble"),
                                Estado = reader.GetBoolean("estado")
                            };
                            lista.Add(p);
                        }
                    }
                }

                return lista;
            


        }

        public int Modificacion(Imagen p)
        {
            int filasAfectadas = 0;
                string sql = @"UPDATE imagen SET
                                url = @url,
                                estado = @estado
                                WHERE ID_imagen = @id";
    
                using (var connection = new MySqlConnection(connectionString))
                {
                    var command = new MySqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@url", p.Url);
                    command.Parameters.AddWithValue("@estado", p.Estado);
                    command.Parameters.AddWithValue("@id", p.IdImagen);
    
                    connection.Open();
                    filasAfectadas = command.ExecuteNonQuery();
                }
    
                return filasAfectadas;
        }

        public int ObtenerCantidad()
        {
            int cantidad = 0;
                string sql = "SELECT COUNT(*) FROM imagen WHERE estado = 1";

                using (var connection = new MySqlConnection(connectionString))
                {
                    var command = new MySqlCommand(sql, connection);
                    connection.Open();
                    cantidad = Convert.ToInt32(command.ExecuteScalar());
                }

                return cantidad;
        }

        public IList<Imagen> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var lista = new List<Imagen>();
            string sql =
                            @"SELECT ID_imagen, url, ID_inmueble, estado
                            FROM imagen
                            WHERE estado = 1
                            ORDER BY ID_imagen
                            LIMIT @tamPagina OFFSET @offset";

                using (var connection = new MySqlConnection(connectionString))
                {
                    var command = new MySqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@offset", (paginaNro - 1) * tamPagina);
                    command.Parameters.AddWithValue("@tampagina", tamPagina);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Imagen p = new Imagen
                            {
                                IdImagen = reader.GetInt32("ID_imagen"),
                                Url = reader.GetString("url"),
                                IdInmueble = reader.GetInt32("ID_inmueble"),
                                Estado = reader.GetBoolean("estado")
                            };
                            lista.Add(p);
                        }
                    }
                    
                }

                return lista;
        }

        public Imagen? ObtenerPorId(int id)
        {
            Imagen? imagen = null;
                string sql = @"SELECT ID_imagen, url, ID_inmueble, estado
                            FROM imagen
                            WHERE ID_imagen = @id";

                using (var connection = new MySqlConnection(connectionString))
                {
                    var command = new MySqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            imagen = new Imagen
                            {
                                IdImagen = reader.GetInt32("ID_imagen"),
                                Url = reader.GetString("url"),
                                IdInmueble = reader.GetInt32("ID_inmueble"),
                                Estado = reader.GetBoolean("estado")
                            };
                        }
                    }
                }

                return imagen;
        }
    }
}