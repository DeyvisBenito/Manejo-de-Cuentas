using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioTiposCuentas
    {
        Task actualizar(TipoCuenta tipoCuenta);
        Task crear(TipoCuenta tipoCuenta);
        Task Eliminar(int id);
        Task<bool> Existe(string Nombre, int UsuarioId);
        Task<IEnumerable<TipoCuenta>> Listar(int UsuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tiposCuentasOrdenar);
    }
    public class RepositorioTiposCuentas: IRepositorioTiposCuentas
    {
        private readonly string conectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            conectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task crear(TipoCuenta tipoCuenta)
        {
            using var conexion = new SqlConnection(conectionString);
            var id = await conexion.QuerySingleAsync<int>("Crear_TipoCuenta", new {Nombre=tipoCuenta.Nombre, UsuarioId=tipoCuenta.UsuarioId},
                                                          commandType: System.Data.CommandType.StoredProcedure);
            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string Nombre, int UsuarioId)
        {
            var conexion = new SqlConnection(conectionString);

            var existe = await conexion.QueryFirstOrDefaultAsync<int>(@"SELECT 1
                                                                        FROM TiposCuentas
                                                                        WHERE Nombre=@Nombre AND UsuarioId=@UsuarioId;",
                                                                        new {Nombre, UsuarioId});

            return existe == 1;

        }

        public async Task<IEnumerable<TipoCuenta>> Listar(int UsuarioId)
        {
            using var conexion = new SqlConnection(conectionString);

            return await conexion.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                            FROM TiposCuentas
                                                            WHERE UsuarioId= @UsuarioId 
                                                            ORDER BY Orden", new {UsuarioId});

            
        }

        public async Task actualizar(TipoCuenta tipoCuenta)
        {
            using var conexion = new SqlConnection(conectionString);
            await conexion.ExecuteAsync(@"UPDATE TiposCuentas
                                            SET Nombre = @Nombre
                                            WHERE Id=@Id ", tipoCuenta);

        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var conexion = new SqlConnection(conectionString);

            return await conexion.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                                        FROM TiposCuentas
                                                                        WHERE Id= @Id AND UsuarioId=@UsuarioId", new {id, usuarioId});
        }

        public async Task Eliminar(int id)
        {
            using var conexion = new SqlConnection(conectionString);

            await conexion.ExecuteAsync(@"DELETE FROM TiposCuentas
                                            WHERE Id=@Id", new {id});
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tiposCuentasOrdenar)
        {
            var query = "UPDATE TiposCuentas SET Orden = @Orden WHERE Id = @Id;";

            using var conexion= new SqlConnection(conectionString);

            await conexion.ExecuteAsync(query, tiposCuentasOrdenar);
        }
    }
}
