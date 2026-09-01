using Microsoft.AspNetCore.Mvc;
using Agencia_inmobiliaria.Models;

namespace Agencia_inmobiliaria.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly IRepositorioInquilino repositorio;

        public InquilinoController(IRepositorioInquilino repositorio)
        {
            this.repositorio = repositorio;
        }

        // GET: Inquilinos
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
                    return View(new List<Inquilino>());
                }
        }
        // GET: Inquilinos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquilinos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino inquilino)
        {
            if (!ModelState.IsValid)
            {
                return View(inquilino);
            }

            try
            {
                inquilino.Estado = true;
                repositorio.Alta(inquilino);
                TempData["success"] = "Inquilino creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                return View(inquilino);
            }
        }

        // GET: Inquilinos/Edit
        public IActionResult Edit(int id)
        {
            try
            {
                var inquilino = repositorio.ObtenerPorId(id);
                if (inquilino == null)
                {
                    return NotFound();
                }
                return View(inquilino);
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo cargar el inquilino. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inquilinos/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inquilino inquilino)
        {
            if (!ModelState.IsValid)
            {
                return View(inquilino);
            }

            try
            {
                inquilino.IdInquilino = id;
                
                int filasAfectadas = repositorio.Modificacion(inquilino);
                if (filasAfectadas > 0)
                {
                    TempData["success"] = "Inquilino modificado exitosamente";
                }
                else
                {
                    TempData["error"] = "No se pudo modificar el inquilino. Verificá que exista.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                return View(inquilino);
            }
        }

        // GET: Inquilinos/Delete
        public IActionResult Delete(int id)
        {
            try
            {
                var inquilino = repositorio.ObtenerPorId(id);
                if (inquilino == null)
                {
                    return NotFound();
                }
                return View(inquilino);
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo cargar el inquilino. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inquilinos/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                repositorio.Baja(id);
                TempData["success"] = "Inquilino borrado exitosamente";
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo eliminar el inquilino. Intente nuevamente.";
            }
            
                return RedirectToAction(nameof(Index));
        }
    }
}