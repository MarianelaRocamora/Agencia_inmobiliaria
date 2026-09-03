using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Agencia_inmobiliaria.Models;

namespace Agencia_inmobiliaria.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioTipoInmueble repositorioTipoInmueble;
        private readonly IRepositorioPropietario repositorioPropietario;

        public InmuebleController(
            IRepositorioInmueble repositorio,
            IRepositorioTipoInmueble repositorioTipoInmueble,
            IRepositorioPropietario repositorioPropietario)
        {
            this.repositorio = repositorio;
            this.repositorioTipoInmueble = repositorioTipoInmueble;
            this.repositorioPropietario = repositorioPropietario;
        }

        private void CargarCombos(Inmueble? inmueble = null)
        {
            var tipos = repositorioTipoInmueble.ObtenerLista(1, repositorioTipoInmueble.ObtenerCantidad());
            var propietarios = repositorioPropietario.ObtenerLista(1, repositorioPropietario.ObtenerCantidad());

            ViewBag.TiposInmueble = new SelectList(tipos, "IdTipoInmueble", "Nombre", inmueble?.IdTipoInmueble);
            ViewBag.Propietarios = new SelectList(
                propietarios.Select(p => new { p.IdPropietario, NombreCompleto = $"{p.Apellido}, {p.Nombre} - DNI: {p.Dni}" }),
                "IdPropietario", "NombreCompleto", inmueble?.IdPropietario);
        }

        // GET inmuebles
        public IActionResult Index(int pagina = 1)
        {
            try
            {
                int tamPagina = 10;
                pagina = Math.Max(pagina, 1);

                var lista = repositorio.ObtenerLista(pagina, tamPagina);
                int totalRegistros = repositorio.ObtenerCantidad();
                int totalPaginas = totalRegistros == 0
                    ? 1
                    : (totalRegistros % tamPagina == 0 ? totalRegistros / tamPagina : totalRegistros / tamPagina + 1);

                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = totalPaginas;

                return View(lista);
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo cargar el listado. Intente nuevamente.";
                return View(new List<Inmueble>());
            }
        }

        // GET inmuebles/Details/5
        public IActionResult Details(int id)
        {
            try
            {
                var inmueble = repositorio.ObtenerPorId(id);
                if (inmueble == null)
                {
                    return NotFound();
                }

                ViewBag.TipoInmueble = repositorioTipoInmueble.ObtenerPorId(inmueble.IdTipoInmueble);
                ViewBag.Propietario = repositorioPropietario.ObtenerPorId(inmueble.IdPropietario);

                return View(inmueble);
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo cargar el inmueble. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET inmuebles/Create
        public IActionResult Create()
        {
            CargarCombos();
            return View();
        }

        // POST inmuebles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos(inmueble);
                return View(inmueble);
            }

            try
            {
                inmueble.Estado = true;
                repositorio.Alta(inmueble);
                TempData["success"] = "Inmueble creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                CargarCombos(inmueble);
                return View(inmueble);
            }
        }

        // GET inmuebles/Edit/5
        public IActionResult Edit(int id)
        {
            try
            {
                var inmueble = repositorio.ObtenerPorId(id);
                if (inmueble == null)
                {
                    return NotFound();
                }
                CargarCombos(inmueble);
                return View(inmueble);
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo cargar el inmueble. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST inmuebles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble inmueble)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos(inmueble);
                return View(inmueble);
            }

            try
            {
                inmueble.IdInmueble = id;

                int filasAfectadas = repositorio.Modificacion(inmueble);
                if (filasAfectadas > 0)
                {
                    TempData["success"] = "Inmueble modificado exitosamente";
                }
                else
                {
                    TempData["error"] = "No se pudo modificar el inmueble. Verificá que exista.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                CargarCombos(inmueble);
                return View(inmueble);
            }
        }

        // GET inmuebles/Delete/intentos
        public IActionResult Delete(int id)
        {
            try
            {
                var inmueble = repositorio.ObtenerPorId(id);
                if (inmueble == null)
                {
                    return NotFound();
                }
                return View(inmueble);
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo cargar el inmueble. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST inmuebles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                repositorio.Baja(id);
                TempData["success"] = "Inmueble borrado exitosamente";
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo eliminar el inmueble. Intente nuevamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}