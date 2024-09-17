using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId);
    }
    public class RepositorioCategorias: IRepositorioCategorias
    {
        private readonly string connectionString;

        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Categoria categoria)
        {
            using var connection= new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int>("crear_categoria", new {Nombre=categoria.Nombre, TipoOperacionId=categoria.TipoOperacionId,
                                                      UsuarioId=categoria.UsuarioId}, commandType: System.Data.CommandType.StoredProcedure);

            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Categoria>("SP_Obtener_Categorias", new { UsuarioId = usuarioId }, 
                                                          commandType: System.Data.CommandType.StoredProcedure);
        }
        
    }
}
