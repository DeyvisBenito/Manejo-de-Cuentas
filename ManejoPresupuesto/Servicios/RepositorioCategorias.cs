using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Actualizar(Categoria categoria);
        Task Borrar(int id);
        Task<Categoria> BuscarPorId(int id, int usuarioId);
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId);
        Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacionId);
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

        public async Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacionId)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Categoria>("SP_ObtenerCategoriasTransacciones", new { UsuarioId = usuarioId, TipoOperacionId=tipoOperacionId },
                                                          commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Categoria> BuscarPorId(int id, int usuarioId)
        {
            using var connection=new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Categoria>("SP_ObtenerCategorioPorID", new {Id=id, UsuarioId=usuarioId},
                                                                        commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Actualizar(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("SP_UpdateCategoria", new {Id=categoria.Id, Nombre=categoria.Nombre, TipoOperacionId=categoria.TipoOperacionId},
                                          commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("SP_BorrarCategoria", new {Id=id}, commandType: System.Data.CommandType.StoredProcedure);
        }
        
    }
}
