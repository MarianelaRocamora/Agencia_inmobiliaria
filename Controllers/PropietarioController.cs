using Microsoft.AspNetCore.Mvc;
using Agencia_inmobiliaria.Models;

namespace Agencia_inmobiliaria.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly IRepositorioPropietario repositorio;

        public PropietarioController(IRepositorioPropietario repositorio)
        {
            this.repositorio = repositorio;
        }

        
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
            return View(new List<Propietario>());
        }
    }

        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario propietario)
        {
            if (!ModelState.IsValid)
            {
                return View(propietario);
            }

            try
            {
                propietario.Estado = true;
                repositorio.Alta(propietario);
                TempData["success"] = "Propietario creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                return View(propietario);
            }
        }

        
        public IActionResult Edit(int id)
        {
            try
            {
                var propietario = repositorio.ObtenerPorId(id);
                if (propietario == null)
                {
                    return NotFound();
                }
                return View(propietario);
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo cargar el propietario. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario propietario)
        {
            if (!ModelState.IsValid)
            {
                return View(propietario);
            }

            try
            {
                propietario.IdPropietario = id;
                int filasAfectadas = repositorio.Modificacion(propietario);
                if (filasAfectadas > 0)
                {
                    TempData["success"] = "Propietario modificado exitosamente";
                }
                else
                {
                    TempData["error"] = "No se pudo modificar el propietario. Verificá que exista.";
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                return View(propietario);
            }
        }

        
        public IActionResult Delete(int id)
        {
            try
            {
                var propietario = repositorio.ObtenerPorId(id);
                if (propietario == null)
                {
                    return NotFound();
                }
                return View(propietario);
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo cargar el propietario. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                repositorio.Baja(id);
                TempData["success"] = "Propietario borrado exitosamente";
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo eliminar el propietario. Intente nuevamente.";
            }
            return RedirectToAction(nameof(Index));
        }

         // GET: /Propietario/Buscar?term=xxx
        // Devuelve coincidencias por nombre, apellido o DNI para usar en combos con búsqueda del lado del servidor.
        [HttpGet]
        public JsonResult Buscar(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                return Json(new List<object>());
            }
 
            var resultados = repositorio.Buscar(term)
                .Select(p => new
                {
                    id = p.IdPropietario,
                    texto = $"{p.Apellido}, {p.Nombre} (DNI {p.Dni})"
                });
 
            return Json(resultados);
        }  
    }
}