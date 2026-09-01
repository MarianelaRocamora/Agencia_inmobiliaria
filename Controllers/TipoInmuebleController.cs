using Microsoft.AspNetCore.Mvc;
using Agencia_inmobiliaria.Models;

namespace Agencia_inmobiliaria.Controllers
{
    public class TipoInmuebleController : Controller
    {
        private readonly IRepositorioTipoInmueble repositorio;

        public TipoInmuebleController(IRepositorioTipoInmueble repositorio)
        {
            this.repositorio = repositorio;
        }

        // GET
        public IActionResult Index()
        {
            try
            {
                var cantidad = repositorio.ObtenerCantidad();
                var lista = repositorio.ObtenerLista(1, cantidad);
                return View(lista);
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo cargar el listado. Intente nuevamente.";
                return View(new List<TipoInmueble>());
            }
        }

        // GET
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TipoInmueble tipoInmueble)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoInmueble);
            }

            try
            {
                tipoInmueble.Estado = true;
                repositorio.Alta(tipoInmueble);
                TempData["success"] = "Tipo de inmueble creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                return View(tipoInmueble);
            }
        }

        // GET: Inquilinos/Edit
        public IActionResult Edit(int id)
        {
            try
            {
                var tipoInmueble = repositorio.ObtenerPorId(id);
                if (tipoInmueble == null)
                {
                    return NotFound();
                }
                return View(tipoInmueble);
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo cargar el tipo de inmueble. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TipoInmueble tipoInmueble)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoInmueble);
            }

            try
            {
                tipoInmueble.IdTipoInmueble = id;
                
                int filasAfectadas = repositorio.Modificacion(tipoInmueble);
                if (filasAfectadas > 0)
                {
                    TempData["success"] = "Tipo de inmueble modificado exitosamente";
                }
                else
                {
                    TempData["error"] = "No se pudo modificar el tipo de inmueble. Verificá que exista.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                return View(tipoInmueble);
            }
        }

        // GET
        public IActionResult Delete(int id)
        {
            try
            {
                var tipoInmueble = repositorio.ObtenerPorId(id);
                if (tipoInmueble == null)
                {
                    return NotFound();
                }
                return View(tipoInmueble);
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo cargar el tipo de inmueble. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                repositorio.Baja(id);
                TempData["success"] = "Tipo de inmueble borrado exitosamente";
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo eliminar el tipo de inmueble. Intente nuevamente.";
            }
            
                return RedirectToAction(nameof(Index));
        }
    }
}