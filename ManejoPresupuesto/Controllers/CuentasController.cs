using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IMapper mapper;
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IServicioReportes servicioReportes;

        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas,
            IMapper mapper, IRepositorioTransacciones repositorioTransacciones, IServicioReportes servicioReportes)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.mapper = mapper;
            this.repositorioTransacciones = repositorioTransacciones;
            this.servicioReportes = servicioReportes;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            var cuentasListadas = await repositorioCuentas.Listar(usuarioId);

            var modelo = cuentasListadas.GroupBy(x => x.TipoCuenta)
                .Select(grupo => new BalanceCuentasViewModel
                {
                    TipoCuenta = grupo.Key,
                    Cuenta = grupo.AsEnumerable()
                }).ToList();


            return View(modelo);
        }

        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            var modelo = new CreacionCuenta();
            modelo.TiposCuentas = await ObtenerTiposCuenta(usuarioId);


            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CreacionCuenta cuenta)
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            var tipoCuenta = repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuenta(usuarioId);
                return View(cuenta);
            }

            await repositorioCuentas.Crear(cuenta);
            
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int Id)
        {
            var usurioId = servicioUsuarios.ObtenerId();
            var cuenta = await repositorioCuentas.BuscarporId(usurioId, Id);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<CreacionCuenta>(cuenta);

            modelo.TiposCuentas= await ObtenerTiposCuenta(usurioId);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CreacionCuenta cuenta)
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuenta(usuarioId);
                return View(cuenta);
            }
            var existe = repositorioCuentas.BuscarporId(usuarioId, cuenta.Id);
            if(existe is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCuentas.Editar(cuenta);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult>Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            var cuenta = await repositorioCuentas.BuscarporId(usuarioId, id);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

             return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int Id)
        {
            var usuarioId = servicioUsuarios.ObtenerId();

            if (!ModelState.IsValid)
            {
               return RedirectToAction("NoEncontrado", "Home");
            }

            var existe = await repositorioCuentas.BuscarporId(usuarioId, Id);
            if(existe is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCuentas.Borrar(Id);

            return RedirectToAction("Index", "Cuentas");
        }

        public async Task<IActionResult> Detalle(int id, int mes, int year)
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            var cuenta = await repositorioCuentas.BuscarporId(usuarioId, id);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            ViewBag.Cuenta = cuenta.Nombre;

            var modelo = await servicioReportes.ObtenerReporteTransaccionesDetalladas(usuarioId, id, mes, year, ViewBag);

            return View(modelo);
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuenta(int usuarioID)
        {
            var tiposCuentas = await repositorioTiposCuentas.Listar(usuarioID);

            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString() ));
        }
    }
}
