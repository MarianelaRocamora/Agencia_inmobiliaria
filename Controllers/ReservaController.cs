using Microsoft.AspNetCore.Mvc;
using Agencia_inmobiliaria.Models;

namespace Agencia_inmobiliaria.Controllers
{
    public class ReservaController : Controller
    {
        private readonly IRepositorioReserva repositorio;
        private readonly IRepositorioInquilino repositorioInquilino;
        private readonly IRepositorioInmueble repositorioInmueble;

        private readonly ILogger<ReservaController> logger;

        public ReservaController(IRepositorioReserva repositorio, IRepositorioInquilino repositorioInquilino, IRepositorioInmueble repositorioInmueble, ILogger<ReservaController> logger)
        {
            this.repositorio = repositorio;
            this.repositorioInquilino = repositorioInquilino;
            this.repositorioInmueble = repositorioInmueble;
            this.logger = logger;
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener el listado de reservas");
                TempData["error"] = "No se pudo cargar el listado. Intente nuevamente.";
                ViewBag.PaginaActual = 1;
                ViewBag.TotalPaginas = 1;
                return View(new List<Reserva>());
            }
        }

        // GET: Reserva/Create
        public IActionResult Create()
        {
            try
            {
                ViewBag.Inquilinos = repositorioInquilino.ObtenerLista(1, 1000);
                return View(new Reserva());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al cargar el formulario de nueva reserva");
                TempData["error"] = "No se pudo cargar el formulario. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Reserva/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reserva reserva)
        {
            if (reserva.FechaEgreso <= reserva.FechaIngreso)
            {
                ModelState.AddModelError("", "La fecha de fin debe ser posterior a la fecha de inicio.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Inquilinos = repositorioInquilino.ObtenerLista(1, 1000);
                return View(reserva);
            }

            try
            {
                // Vuelvo a validar disponibilidad en el servidor, por si cambió entre la búsqueda y el submit
                var disponibles = repositorioInmueble.ObtenerDisponiblesEntreFechas(reserva.FechaIngreso, reserva.FechaEgreso);
                if (!disponibles.Any(i => i.IdInmueble == reserva.IdInmueble))
                {
                    ModelState.AddModelError("", "Ese inmueble ya no está disponible en esas fechas. Elegí otro.");
                    ViewBag.Inquilinos = repositorioInquilino.ObtenerLista(1, 1000);
                    return View(reserva);
                }

                repositorio.Alta(reserva);
                TempData["success"] = "Reserva creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear la reserva");
                ModelState.AddModelError("", "No se pudo guardar la reserva. Intente nuevamente.");
                ViewBag.Inquilinos = repositorioInquilino.ObtenerLista(1, 1000);
                return View(reserva);
            }
        }

        // GET: Reserva/Cancelar/:id
        public IActionResult Cancelar(int id)
        {
            try
            {
                var reserva = repositorio.ObtenerPorId(id);
                if (reserva == null) return NotFound();

                if (reserva.FechaCancelacion is not null)
                {
                    TempData["error"] = "Esta reserva ya fue cancelada.";
                    return RedirectToAction(nameof(Index));
                }

                return View(reserva);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener la reserva para cancelar (Id: {Id})", id);
                TempData["error"] = "No se pudo cargar la reserva. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Reserva/Edit/:id
        public IActionResult Edit(int id)
        {
            try
            {
                var reserva = repositorio.ObtenerPorId(id);
                if (reserva == null) return NotFound();

                ViewBag.Inquilinos = repositorioInquilino.ObtenerLista(1, 1000);
                return View(reserva);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al cargar la reserva para editar (Id: {Id})", id);
                TempData["error"] = "No se pudo cargar la reserva. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Reserva/Edit/:id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Reserva reserva)
        {
            if (id != reserva.IdReserva) return NotFound();

            if (reserva.FechaEgreso <= reserva.FechaIngreso)
            {
                ModelState.AddModelError("", "La fecha de fin debe ser posterior a la fecha de inicio.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Inquilinos = repositorioInquilino.ObtenerLista(1, 1000);
                return View(reserva);
            }

            try
            {
                // Revalida disponibilidad, excluyendo esta misma reserva del chequeo
                var disponibles = repositorioInmueble.ObtenerDisponiblesEntreFechas(reserva.FechaIngreso, reserva.FechaEgreso, id);
                if (!disponibles.Any(i => i.IdInmueble == reserva.IdInmueble))
                {
                    ModelState.AddModelError("", "Ese inmueble ya no está disponible en esas fechas. Elegí otro.");
                    ViewBag.Inquilinos = repositorioInquilino.ObtenerLista(1, 1000);
                    return View(reserva);
                }

                int filasAfectadas = repositorio.Modificacion(reserva);

                if (filasAfectadas > 0)
                {
                    TempData["success"] = "Reserva modificada exitosamente";
                }
                else
                {
                    TempData["error"] = "No se pudo modificar la reserva. Verificá que exista.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al modificar la reserva (Id: {Id})", id);
                ModelState.AddModelError("", "No se pudo guardar los cambios. Intente nuevamente.");
                ViewBag.Inquilinos = repositorioInquilino.ObtenerLista(1, 1000);
                return View(reserva);
            }
        }

        // GET: Reserva/Delete/:id
        public IActionResult Delete(int id)
        {
            try
            {
                var reserva = repositorio.ObtenerPorId(id);
                if (reserva == null) return NotFound();
                return View(reserva);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener la reserva para eliminar (Id: {Id})", id);
                TempData["error"] = "No se pudo cargar la reserva. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Reserva/Delete/:id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                int filasAfectadas = repositorio.Baja(id);

                if (filasAfectadas > 0)
                {
                    TempData["success"] = "Reserva eliminada exitosamente";
                }
                else
                {
                    TempData["error"] = "No se pudo eliminar la reserva. Puede que ya no exista.";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al eliminar la reserva (Id: {Id})", id);
                TempData["error"] = "No se pudo eliminar la reserva. Intente nuevamente.";
            }

            return RedirectToAction(nameof(Index));
        }

         // GET: Reserva/Details/5
        public IActionResult Details(int id)
        {
            try
            {
                var reserva = repositorio.ObtenerPorId(id);
                if (reserva == null) return NotFound();
                return View(reserva);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener el detalle de la reserva (Id: {Id})", id);
                TempData["error"] = "No se pudo cargar la reserva. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Reserva/Cancelar/:id
        [HttpPost, ActionName("Cancelar")]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarConfirmado(int id, DateTime fechaCancelacion, DateTime fechaIngreso, DateTime fechaEgreso)
        {
            if (fechaCancelacion.Date < fechaIngreso.Date || fechaCancelacion.Date >= fechaEgreso.Date)
            {
                TempData["error"] = "La fecha de cancelación debe estar entre el inicio y el fin original de la reserva.";
                return RedirectToAction(nameof(Cancelar), new { id });
            }

            try
            {
                int filasAfectadas = repositorio.Cancelar(id, fechaCancelacion);

                if (filasAfectadas > 0)
                {
                    TempData["success"] = "Reserva cancelada exitosamente";
                }
                else
                {
                    TempData["error"] = "No se pudo cancelar la reserva. Puede que ya esté cancelada.";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al cancelar la reserva (Id: {Id})", id);
                TempData["error"] = "No se pudo cancelar la reserva. Intente nuevamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Reserva/Extender/:id
        public IActionResult Extender(int id)
        {
            try
            {
                var reservaOriginal = repositorio.ObtenerPorId(id);
                if (reservaOriginal == null) return NotFound();

                if (reservaOriginal.FechaCancelacion is not null)
                {
                    TempData["error"] = "No se puede extender una reserva cancelada.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                
                var nuevaReserva = new Reserva
                {
                    IdInquilino = reservaOriginal.IdInquilino,
                    IdInmueble = reservaOriginal.IdInmueble,
                    Inquilino = reservaOriginal.Inquilino,
                    Inmueble = reservaOriginal.Inmueble,
                    FechaIngreso = reservaOriginal.FechaEgreso,  
                    MontoDia = reservaOriginal.MontoDia
                };

                ViewBag.ReservaOriginalId = reservaOriginal.IdReserva;
                return View(nuevaReserva);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al preparar la extensión de la reserva (Id: {Id})", id);
                TempData["error"] = "No se pudo cargar la reserva. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Reserva/Extender/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Extender(int idReservaOriginal, Reserva reserva)
        {
            if (reserva.FechaEgreso <= reserva.FechaIngreso)
            {
                ModelState.AddModelError("", "La fecha de fin debe ser posterior a la fecha de inicio.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ReservaOriginalId = idReservaOriginal;
                return View(reserva);
            }

            try
            {
                bool disponible = repositorioInmueble.InmuebleDisponibleEntreFechas(
                    reserva.IdInmueble, reserva.FechaIngreso, reserva.FechaEgreso);

                if (!disponible)
                {
                    ModelState.AddModelError("", "El inmueble ya tiene otra reserva en ese período.");
                    ViewBag.ReservaOriginalId = idReservaOriginal;
                    return View(reserva);
                }

                repositorio.Alta(reserva);
                TempData["success"] = "Reserva extendida exitosamente (se creó un nuevo alquiler)";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear la extensión de la reserva (IdOriginal: {Id})", idReservaOriginal);
                ModelState.AddModelError("", "No se pudo guardar la extensión. Intente nuevamente.");
                ViewBag.ReservaOriginalId = idReservaOriginal;
                return View(reserva);
            }
        }
    }
}