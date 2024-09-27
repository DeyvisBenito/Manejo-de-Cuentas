namespace ManejoPresupuesto.Models
{
    public class ReporteTransaccionesDetalladas
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public IEnumerable<TransaccionesDetalladas> TransaccionesAgrupadas { get; set; }
        public decimal BalanceGastos => TransaccionesAgrupadas.Sum(x => x.BalanceGasto);
        public decimal BalanceIngreso => TransaccionesAgrupadas.Sum(x => x.BalanceIngreso);
        public decimal Total => BalanceIngreso - BalanceGastos;

        public class TransaccionesDetalladas
        {
            public DateTime FechaTransaccion { get; set; }
            public IEnumerable<Transaccion> Transacciones { get; set; }
            public decimal BalanceIngreso => Transacciones.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso).Sum(x => x.Monto);
            public decimal BalanceGasto => Transacciones.Where(x => x.TipoOperacionId == TipoOperacion.Gasto).Sum(x => x.Monto);
        }
    }
}
