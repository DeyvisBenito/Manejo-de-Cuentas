using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCuentas
    {
        Task Borrar(int id);
        Task<Cuenta> BuscarporId(int usuarioId, int cuentaId);
        Task Crear(Cuenta cuenta);
        Task Editar(CreacionCuenta cuenta);
        Task<IEnumerable<Cuenta>> Listar(int usuarioId);
    }
    public class RepositorioCuentas: IRepositorioCuentas
    {
        private readonly string connectionString;

        public RepositorioCuentas(IConfiguration configuration)
        {
            
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var conexion= new SqlConnection(connectionString);

            var id = await conexion.QuerySingleAsync<int>("CREAR_CUENTA", new {Nombre=cuenta.Nombre, TipoCuentaId=cuenta.TipoCuentaId, Balance=cuenta.Balance,
                                                                Descripcion=cuenta.Descripcion}, commandType: System.Data.CommandType.StoredProcedure);

            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> Listar(int usuarioId)
        {
            using var conexion = new SqlConnection(connectionString);

            return await conexion.QueryAsync<Cuenta>("LISTAR_CUENTAS", new { usuarioId }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Cuenta> BuscarporId(int usuarioId, int cuentaId)
        {
            using var conexion = new SqlConnection(connectionString);

            return await conexion.QueryFirstOrDefaultAsync<Cuenta>("BuscarPorId", new {UsuarioId=usuarioId, CuentaId=cuentaId},
                                                                   commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Editar(CreacionCuenta cuenta)
        {
            using var conexion = new SqlConnection(connectionString);

            await conexion.ExecuteAsync("Editar_Cuenta", new {Nombre=cuenta.Nombre, TipoCuentaId=cuenta.TipoCuentaId, Balance=cuenta.Balance, Descripcion=cuenta.Descripcion, CuentaId=cuenta.Id},
                                    commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("Borrar_cuenta", new { Id = id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
