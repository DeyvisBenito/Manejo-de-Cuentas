namespace ManejoPresupuesto.Models
{
    public class ReporteSemanalViewModel
    {
        public IEnumerable<ReporteTransaccionesPorSemana> TransaccionesPorSemanas { get; set; }
        public DateTime FechaReferencia { get; set; }
        public decimal Ingresos => TransaccionesPorSemanas.Sum(x=>x.Ingresos);
        public decimal Gastos => TransaccionesPorSemanas.Sum(x=>x.Gastos);
        public decimal Total => Ingresos - Gastos; 


    }
}
