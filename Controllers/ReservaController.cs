using Microsoft.AspNetCore.Mvc;
using Agencia_inmobiliaria.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Agencia_inmobiliaria.Controllers
{
    [Authorize]
    public class ReservaController : Controller
    {
        private readonly IRepositorioReserva repositorio;
        private readonly IRepositorioInquilino repositorioInquilino;
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioPago repositorioPago;

        private readonly ILogger<ReservaController> logger;


       public ReservaController(IRepositorioReserva repositorio, IRepositorioInquilino repositorioInquilino, IRepositorioInmueble repositorioInmueble, IRepositorioPago repositorioPago, ILogger<ReservaController> logger)
        {
            this.repositorio = repositorio;
            this.repositorioInquilino = repositorioInquilino;
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioPago = repositorioPago;
            this.logger = logger;
        }

        [AllowAnonymous]
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
             if (reserva.FechaIngreso.Date < DateTime.Today)
            {
                ModelState.AddModelError("", "La fecha de ingreso no puede ser anterior a hoy.");
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

                reserva.IdUsuarioCreador = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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

                ViewBag.Pagos = repositorioPago.ObtenerPorReserva(id);

                if (reserva.Inmueble != null)
                {
                    decimal totalReserva = reserva.MontoDia * (decimal)(reserva.FechaEgreso - reserva.FechaIngreso).Days;
                    ViewBag.SenaSugerida = totalReserva * (reserva.Inmueble.PorcentajeReserva / 100);
                }

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
        // Recalcula todo contra la reserva real (no confía en fechas ocultas del form),
        // calcula la multa por finalización anticipada y la carga como Pago en la misma operación.
        [HttpPost, ActionName("Cancelar")]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarConfirmado(int id, DateTime fechaCancelacion)
        {
            Reserva? reserva;
            try
            {
                reserva = repositorio.ObtenerPorId(id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener la reserva para cancelar (Id: {Id})", id);
                TempData["error"] = "No se pudo cargar la reserva. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }

            if (reserva == null) return NotFound();

            if (reserva.FechaCancelacion is not null)
            {
                TempData["error"] = "Esta reserva ya fue cancelada.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (fechaCancelacion.Date < reserva.FechaIngreso.Date || fechaCancelacion.Date >= reserva.FechaEgreso.Date)
            {
                TempData["error"] = "La fecha de cancelación debe estar entre el inicio y el fin original de la reserva.";
                return RedirectToAction(nameof(Cancelar), new { id });
            }

            int idUsuarioActual = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                int filasAfectadas = repositorio.Cancelar(id, fechaCancelacion, idUsuarioActual);

                if (filasAfectadas > 0)
                {
                    decimal multa = CalcularMulta(reserva, fechaCancelacion);

                    var pagoMulta = new Pago
                    {
                        Concepto = "Multa por finalización anticipada",
                        FechaPago = DateTime.Today,
                        Importe = multa,
                        IdReserva = id,
                        IdUsuarioCreador = idUsuarioActual, 
                        Estado = true
                    };
                    repositorioPago.Alta(pagoMulta);

                    TempData["success"] = $"Reserva cancelada exitosamente. Se generó una multa de {multa.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-US"))} en los pagos de la reserva.";
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

            return RedirectToAction(nameof(Details), new { id });
        }

        // Calcula la multa por finalización anticipada 
        // si se cumplió menos de la mitad del tiempo original, 50% del monto restante de alquiler;
        // caso contrario, 25% del monto restante.
        private static decimal CalcularMulta(Reserva reserva, DateTime fechaCancelacion)
        {
            int diasTotales = (reserva.FechaEgreso - reserva.FechaIngreso).Days;
            int diasCumplidos = (fechaCancelacion.Date - reserva.FechaIngreso.Date).Days;
            int diasRestantes = Math.Max(diasTotales - diasCumplidos, 0);

            decimal montoRestante = reserva.MontoDia * diasRestantes;
            decimal porcentaje = diasCumplidos < diasTotales / 2.0 ? 0.50m : 0.25m;

            return Math.Round(montoRestante * porcentaje, 2);
        }

        // GET: Reserva/Extender/:id
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

                // Se piden directo a sus propios repositorios para garantizar que Inquilino
                // e Inmueble vengan siempre completos (con % de reserva incluido).
                var inquilino = repositorioInquilino.ObtenerPorId(reservaOriginal.IdInquilino);
                var inmueble = repositorioInmueble.ObtenerPorId(reservaOriginal.IdInmueble);

                var nuevaReserva = new Reserva
                {
                    IdInquilino = reservaOriginal.IdInquilino,
                    IdInmueble = reservaOriginal.IdInmueble,
                    Inquilino = inquilino,
                    Inmueble = inmueble,
                    FechaIngreso = reservaOriginal.FechaEgreso,
                    MontoDia = inmueble?.PrecioDia ?? reservaOriginal.MontoDia
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
            if (reserva.FechaIngreso.Date < DateTime.Today)
            {
                ModelState.AddModelError("", "La fecha de ingreso no puede ser anterior a hoy.");
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Vigentes(int pagina = 1)
        {
            try
            {
                int tamPagina = 10;
                pagina = Math.Max(pagina, 1);

                var lista = repositorio.ObtenerVigentes(pagina, tamPagina);
                int totalRegistros = repositorio.ObtenerCantidadVigentes();
                int totalPaginas = totalRegistros == 0
                    ? 1
                    : (totalRegistros % tamPagina == 0 ? totalRegistros / tamPagina : totalRegistros / tamPagina + 1);

                var datos = lista.Select(r => new
                {
                    id = r.IdReserva,
                    inquilino = $"{r.Inquilino?.Nombre} {r.Inquilino?.Apellido}",
                    inmueble = r.Inmueble?.Direccion,
                    fechaInicio = r.FechaIngreso.ToString("dd/MM/yyyy"),
                    fechaFin = r.FechaEgreso.ToString("dd/MM/yyyy"),
                    montoDiario = r.MontoDia.ToString("C"),
                    fechaCancelacion = r.FechaCancelacion?.ToString("dd/MM/yyyy")
                });

                return Json(new { reservas = datos, paginaActual = pagina, totalPaginas });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener las reservas vigentes");
                return Json(new { reservas = new List<object>(), paginaActual = 1, totalPaginas = 1 });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult TerminanEn(int dias = 7, int pagina = 1)
        {
            try
            {
                int tamPagina = 10;
                pagina = Math.Max(pagina, 1);
                dias = Math.Max(dias, 1);

                var lista = repositorio.ObtenerQueTerminanEn(dias, pagina, tamPagina);
                int totalRegistros = repositorio.ObtenerCantidadQueTerminanEn(dias);
                int totalPaginas = totalRegistros == 0
                    ? 1
                    : (totalRegistros % tamPagina == 0 ? totalRegistros / tamPagina : totalRegistros / tamPagina + 1);

                var datos = lista.Select(r => new
                {
                    id = r.IdReserva,
                    inquilino = $"{r.Inquilino?.Nombre} {r.Inquilino?.Apellido}",
                    inmueble = r.Inmueble?.Direccion,
                    fechaInicio = r.FechaIngreso.ToString("dd/MM/yyyy"),
                    fechaFin = r.FechaEgreso.ToString("dd/MM/yyyy"),
                    montoDiario = r.MontoDia.ToString("C")
                });

                return Json(new { reservas = datos, paginaActual = pagina, totalPaginas });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener reservas que terminan en {Dias} días", dias);
                return Json(new { reservas = new List<object>(), paginaActual = 1, totalPaginas = 1 });
            }
        }
    }
}