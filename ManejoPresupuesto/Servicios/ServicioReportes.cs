using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{

    public interface IServicioReportes
    {
        Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladas(int usuarioId, int cuentaId, int mes, int year, dynamic ViewBag);
        Task<IEnumerable<ReporteTransaccionesPorSemana>> ObtenerReporteTransaccionesPorSemana(int usuarioId, int mes, int year, dynamic ViewBag);
        Task<ReporteTransaccionesDetalladas> ObtenerTransaccionesDetalladas(int usuarioId, int mes, int year, dynamic ViewBag);
    }

    public class ServicioReportes: IServicioReportes
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly HttpContext httpContext;

        public ServicioReportes(IRepositorioTransacciones repositorioTransacciones, IHttpContextAccessor httpContextAccessor)
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<ReporteTransaccionesDetalladas> ObtenerTransaccionesDetalladas(int usuarioId, int mes, int year, dynamic ViewBag)
        {

            (DateTime FechaInicio, DateTime FechaFin) = GenerarFechaInicioYFin(mes, year);

            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                UsuarioId = usuarioId,
                FechaInicio = FechaInicio,
                FechaFin = FechaFin
            };

            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioID(parametro);

            var modelo = GenerarReporteTransaccionesDetalladas(FechaInicio, FechaFin, transacciones);
            AsignarValoresAlViewBag(ViewBag, FechaInicio);

            return modelo;
        }

        public async Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladas(int usuarioId, int cuentaId, int mes, int year, dynamic ViewBag)
        {
            (DateTime FechaInicio, DateTime FechaFin) = GenerarFechaInicioYFin(mes, year);

            var TransaccionPorCuenta = new ObtenerTransaccionesPorCuenta()
            {
                UsuarioId = usuarioId,
                CuentaId = cuentaId,
                FechaInicio = FechaInicio,
                FechaFin = FechaFin
            };

            var ObtenerTransaccionesPorCuenta = await repositorioTransacciones.ObtenerPorCuentaId(TransaccionPorCuenta);
            var modelo = GenerarReporteTransaccionesDetalladas(FechaInicio, FechaFin, ObtenerTransaccionesPorCuenta);

            AsignarValoresAlViewBag(ViewBag, FechaInicio);

            return modelo;
        }

        public async Task<IEnumerable<ReporteTransaccionesPorSemana>> ObtenerReporteTransaccionesPorSemana(int usuarioId, int mes, int year, dynamic ViewBag)
        {
            (DateTime FechaInicio, DateTime FechaFin) = GenerarFechaInicioYFin(mes, year);

            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                UsuarioId = usuarioId,
                FechaInicio = FechaInicio,
                FechaFin = FechaFin
            };

            AsignarValoresAlViewBag(ViewBag, FechaInicio);

            var modelo = await repositorioTransacciones.ObtenerTransaccionesPorSemana(parametro);
            return modelo;
        }

        private void AsignarValoresAlViewBag(dynamic ViewBag, DateTime FechaInicio)
        {
            ViewBag.mesAnterior = FechaInicio.AddMonths(-1).Month;
            ViewBag.yearAnterior = FechaInicio.AddMonths(-1).Year;
            ViewBag.mesPosterior = FechaInicio.AddMonths(1).Month;
            ViewBag.yearPosterior = FechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = httpContext.Request.Path + httpContext.Request.QueryString;
        }

        private static ReporteTransaccionesDetalladas GenerarReporteTransaccionesDetalladas(DateTime FechaInicio, DateTime FechaFin, IEnumerable<Transaccion> ObtenerTransaccionesPorCuenta)
        {
            var modelo = new ReporteTransaccionesDetalladas();


            var transaccionesPorFecha = ObtenerTransaccionesPorCuenta.OrderByDescending(x => x.FechaTransaccion)
                .GroupBy(x => x.FechaTransaccion).Select(grupo => new ReporteTransaccionesDetalladas.TransaccionesDetalladas()
                {
                    FechaTransaccion = grupo.Key,
                    Transacciones = grupo.AsEnumerable()
                });

            modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = FechaInicio;
            modelo.FechaFin = FechaFin;
            return modelo;
        }

        private (DateTime fechaInicio, DateTime fechaFin) GenerarFechaInicioYFin(int mes, int year)
        {
            DateTime FechaInicio;
            DateTime FechaFin;

            if (mes > 12 || mes <= 0 || year <= 1900)
            {
                DateTime hoy = DateTime.Today;
                FechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                FechaInicio = new DateTime(year, mes, 1);
            }

            FechaFin = FechaInicio.AddMonths(1).AddDays(-1);

            return (FechaInicio, FechaFin);
        }

    }
}
