namespace ManejoPresupuesto.Models
{
    public class ReporteMensualViewModel
    {
        public IEnumerable<ResultadoObtenerPorMes> TransaccionesPorMes { get; set; }
        public decimal Ingreso => TransaccionesPorMes.Sum(x => x.Ingreso);
        public decimal Gasto => TransaccionesPorMes.Sum(x => x.Gasto);
        public decimal Total => Ingreso - Gasto;
        public int Year { get; set; }
    }
}
