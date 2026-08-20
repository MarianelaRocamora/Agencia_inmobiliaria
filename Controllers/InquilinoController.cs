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
        public IActionResult Index()
        {
            var lista = repositorio.ObtenerLista(1, 100);
            return View(lista);
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
            var inquilino = repositorio.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
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
                repositorio.Modificacion(inquilino);
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
            var inquilino = repositorio.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        // POST: Inquilinos/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}