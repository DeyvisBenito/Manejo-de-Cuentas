using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioID(ParametroObtenerTransaccionesPorUsuario parametro);
        Task<IEnumerable<ReporteTransaccionesPorSemana>> ObtenerTransaccionesPorSemana(ParametroObtenerTransaccionesPorUsuario model);
    }
    public class RepositorioTransacciones: IRepositorioTransacciones
    {
        private readonly string connectionString;
        public RepositorioTransacciones(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);

            var id = await connection.QueryFirstAsync<int>("SP_CrearTransaccion", new {transaccion.UsuarioId, transaccion.FechaTransaccion, transaccion.Monto,
            transaccion.CategoriaId, transaccion.CuentaId, transaccion.Nota}, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("SP_ACTUALIZAR_TRANSACCION", new
            {
                transaccion.Id,
                transaccion.FechaTransaccion,
                transaccion.Monto,
                transaccion.CuentaId,
                transaccion.CategoriaId,
                transaccion.Nota,
                montoAnterior,
                cuentaAnteriorId
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Transaccion>("SP_Buscar_Transaccion_PorId",new
            {
                id, usuarioId
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Borrar(int id)
        {
            using var connection=new SqlConnection(connectionString);

            await connection.ExecuteAsync("SP_Borrar_Transaccion", new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Transaccion>("SP_Transacciones_Por_Cuentas", new
            {
                modelo.UsuarioId,
                modelo.CuentaId,
                modelo.FechaInicio,
                modelo.FechaFin
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioID(ParametroObtenerTransaccionesPorUsuario parametro)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Transaccion>("SP_Obtener_Transaccion_PorUsuarioId", new
            {
                parametro.UsuarioId, parametro.FechaInicio, parametro.FechaFin
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ReporteTransaccionesPorSemana>> ObtenerTransaccionesPorSemana(ParametroObtenerTransaccionesPorUsuario model)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<ReporteTransaccionesPorSemana>("SP_Transacciones_Semanales", new
            {
                model.UsuarioId,
                model.FechaInicio,
                model.FechaFin
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        
    }
}
