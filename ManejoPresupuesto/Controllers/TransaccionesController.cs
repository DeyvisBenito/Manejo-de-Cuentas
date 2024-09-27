using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IMapper mapper;

        public TransaccionesController(IServicioUsuarios servicioUsuarios, IRepositorioTransacciones repositorioTransacciones, 
            IRepositorioCuentas repositorioCuentas, IRepositorioCategorias repositorioCategorias, IMapper mapper)
        {
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioTransacciones = repositorioTransacciones;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioCategorias = repositorioCategorias;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            var modelo = new TransaccionCreacionViewModel();

            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo)
        {
            var usuarioId = servicioUsuarios.ObtenerId();

            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);

                return View(modelo);
            }

            var cuentaExiste = await repositorioCuentas.BuscarporId(usuarioId, modelo.CuentaId);

            if (cuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoriaExiste = await repositorioCategorias.BuscarPorId(modelo.CategoriaId, usuarioId);

            if(categoriaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            modelo.UsuarioId = usuarioId;

            if(modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.Monto *= -1;
            }

            await repositorioTransacciones.Crear(modelo);

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Editar(int Id, string UrlRetorno = null)
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            var transaccion = await repositorioTransacciones.ObtenerPorId(Id, usuarioId);

            if(transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var modelo = mapper.Map<TransaccionActualizacionViewModel>(transaccion);

            modelo.MontoAnterior = modelo.Monto;

            if(modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.MontoAnterior = modelo.Monto * -1; ;
            }

            modelo.CuentaAnteriorId = transaccion.CuentaId;
            modelo.Categorias = await ObtenerCategorias(usuarioId, transaccion.TipoOperacionId);
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.UrlRetorno = UrlRetorno;

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionActualizacionViewModel modelo)
        {
            var usuarioId = servicioUsuarios.ObtenerId();

            if (!ModelState.IsValid)
            {
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                return View(modelo);
            }

            var cuentaExiste = await repositorioCuentas.BuscarporId(usuarioId, modelo.CuentaId);
            if(cuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoriaExiste = await repositorioCategorias.BuscarPorId(modelo.CategoriaId, usuarioId);
            if(categoriaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var transaccion = mapper.Map<Transaccion>(modelo);

            if(transaccion.TipoOperacionId == TipoOperacion.Gasto)
            {
                transaccion.Monto *= -1;
            }

            await repositorioTransacciones.Actualizar(transaccion, modelo.MontoAnterior, modelo.CuentaAnteriorId);

            if (string.IsNullOrEmpty(modelo.UrlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(modelo.UrlRetorno);
            }

            
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id, string urlRetorno=null)
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id, usuarioId);

            if(transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTransacciones.Borrar(id);

            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlRetorno);
            }

        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
        {
            var cuentas = await repositorioCuentas.Listar(usuarioId);

            return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacion)
        {
            var categorias = await repositorioCategorias.ObtenerCategorias(usuarioId, tipoOperacion);
            return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
            return Ok(categorias);
        }
    }
}
