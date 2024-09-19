using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IServicioUsuarios servicioUsuarios;

        public CategoriasController(IRepositorioCategorias repositorioCategorias, IServicioUsuarios servicioUsuarios)
        {
            this.repositorioCategorias = repositorioCategorias;
            this.servicioUsuarios = servicioUsuarios;
        }


        public async Task<IActionResult> Index()
        {
            var UsuarioId = servicioUsuarios.ObtenerId();

            var categorias = await repositorioCategorias.ObtenerCategorias(UsuarioId);
            return View(categorias);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            var usuarioId = servicioUsuarios.ObtenerId();

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            categoria.UsuarioId = usuarioId;
            await repositorioCategorias.Crear(categoria);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerId();

            var categoriaExiste = await repositorioCategorias.BuscarPorId(id, usuarioId);

            if(categoriaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoriaExiste);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoria)
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            if (!ModelState.IsValid)
            {
                return View(categoria); 
            }
            categoria.UsuarioId= usuarioId;
            var categoriaExiste = await repositorioCategorias.BuscarPorId(categoria.Id, categoria.UsuarioId);

            if(categoriaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCategorias.Actualizar(categoria);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerId();
            var existe = repositorioCategorias.BuscarPorId(id, usuarioId);

            if(existe is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCategorias.Borrar(id);

            return RedirectToAction("Index");
        }
    }
}
