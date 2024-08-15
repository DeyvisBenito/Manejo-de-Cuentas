namespace ManejoPresupuesto.Servicios
{
    public interface IServicioUsuarios
    {
        int ObtenerId();
    }
    public class ServicioUsuarios: IServicioUsuarios
    {
        public int ObtenerId()
        {
            return 1;
        }
    }
}
