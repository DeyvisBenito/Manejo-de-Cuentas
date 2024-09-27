using AutoMapper;
using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Cuenta, CreacionCuenta>();
            CreateMap<TransaccionActualizacionViewModel, Transaccion>().ReverseMap();
        }
    }
}
