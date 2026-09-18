using Microsoft.AspNetCore.Mvc;
using Agencia_inmobiliaria.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Agencia_inmobiliaria.Controllers
{
    public class PagoController : Controller
    {
        private readonly IRepositorioPago repositorio;
        private readonly IRepositorioReserva repositorioReserva;
        private readonly ILogger<PagoController> logger;

        public PagoController(IRepositorioPago repositorio, IRepositorioReserva repositorioReserva, ILogger<PagoController> logger)
        {
            this.repositorio = repositorio;
            this.repositorioReserva = repositorioReserva;
            this.logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "No se pudo registrar el pago. Verificá los datos ingresados.";
                return RedirectToAction("Details", "Reserva", new { id = pago.IdReserva });
            }

            try
            {
                var reserva = repositorioReserva.ObtenerPorId(pago.IdReserva);
                if (reserva == null)
                {
                    TempData["error"] = "La reserva no existe.";
                    return RedirectToAction("Index", "Reserva");
                }
                pago.IdUsuarioCreador = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                repositorio.Alta(pago);
                TempData["success"] = "Pago registrado exitosamente.";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al registrar el pago (IdReserva: {IdReserva})", pago.IdReserva);
                TempData["error"] = "No se pudo registrar el pago. Intente nuevamente.";
            }

            return RedirectToAction("Details", "Reserva", new { id = pago.IdReserva });
        }

        public IActionResult Edit(int id)
        {
            try
            {
                var pago = repositorio.ObtenerPorId(id);
                if (pago == null) return NotFound();
                return View(pago);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al cargar el pago (Id: {Id})", id);
                TempData["error"] = "No se pudo cargar el pago. Intente nuevamente.";
                return RedirectToAction("Index", "Reserva");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Pago pago)
        {
            if (id != pago.IdPago) return NotFound();

            if (string.IsNullOrWhiteSpace(pago.Concepto) || pago.Concepto.Length < 2 || pago.Concepto.Length > 100)
            {
                ModelState.AddModelError(nameof(pago.Concepto), "El concepto debe tener entre 2 y 100 caracteres");
            }

            if (!ModelState.IsValid)
            {
                return View(pago);
            }

            try
            {
                int filasAfectadas = repositorio.Modificacion(pago);
                if (filasAfectadas > 0)
                {
                    TempData["success"] = "Concepto actualizado exitosamente.";
                }
                else
                {
                    TempData["error"] = "No se pudo modificar el pago. Verificá que exista.";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al modificar el pago (Id: {Id})", id);
                TempData["error"] = "No se pudo modificar el pago. Intente nuevamente.";
                return View(pago);
            }

            return RedirectToAction("Details", "Reserva", new { id = pago.IdReserva });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Anular(int id, int idReserva)
        {
            int idUsuarioActual = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                int filasAfectadas = repositorio.Baja(id, idUsuarioActual);
                if (filasAfectadas > 0)
                {
                    TempData["success"] = "Pago anulado exitosamente.";
                }
                else
                {
                    TempData["error"] = "No se pudo anular el pago.";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al anular el pago (Id: {Id})", id);
                TempData["error"] = "No se pudo anular el pago. Intente nuevamente.";
            }

            return RedirectToAction("Details", "Reserva", new { id = idReserva });
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Details(int id)
        {
            try
            {
                var pago = repositorio.ObtenerPorId(id);
                if (pago == null) return NotFound();
                return View(pago);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener el detalle del pago (Id: {Id})", id);
                TempData["error"] = "No se pudo cargar el pago. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}