using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;

        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            var lista = await repositorioTiposCuentas.Listar(usuarioId);

            return View(lista);
        }

        public IActionResult Crear()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {

            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }

            tipoCuenta.UsuarioId = servicioUsuarios.ObtenerId();

            var existe = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

            if (existe)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe.");
                return View(tipoCuenta);
            }

            await repositorioTiposCuentas.crear(tipoCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> existe(string Nombre)
        {
            var usuarioId = servicioUsuarios.ObtenerId();

            var yaExiste = await repositorioTiposCuentas.Existe(Nombre, usuarioId);

            if (yaExiste)
            {
                return Json($"El nombre {Nombre} ya existe.");
            }

            return Json(true);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            var obtenerPorId = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);


            if (obtenerPorId is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(obtenerPorId);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }

            var usuarioId = servicioUsuarios.ObtenerId();

            var existe = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, usuarioId);
            if (existe)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El tipo de cuenta {tipoCuenta.Nombre} ya existe.");
                return View(tipoCuenta);
            }

            var obtenerPorId = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

            if (obtenerPorId is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTiposCuentas.actualizar(tipoCuenta);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            var obtenerPorId = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (obtenerPorId is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(obtenerPorId);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarRegistro(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            var obtenerPorId = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (obtenerPorId is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTiposCuentas.Eliminar(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var idUsuario = servicioUsuarios.ObtenerId();
            var cuentasUsuario= await repositorioTiposCuentas.Listar(idUsuario);
            var idTiposCuentasUsuario = cuentasUsuario.Select(x => x.Id);

            var existeAlgunaDistinta = ids.Except(idTiposCuentasUsuario).ToList();

            if (existeAlgunaDistinta.Count() > 0)
            {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((valor, indice) =>
            new TipoCuenta() { Id = valor, Orden = indice + 1 }).AsEnumerable();

            await repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);


            return Ok();
        }
    }
}