namespace ManejoPresupuesto.Models
{
    public class BalanceCuentasViewModel
    {
        public string TipoCuenta { get; set; }
        public IEnumerable<Cuenta> Cuenta { get; set; }
        public decimal Balance => Cuenta.Sum(x => x.Balance);
    }
}
