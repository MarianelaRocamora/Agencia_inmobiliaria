using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Agencia_inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
namespace Agencia_inmobiliaria.Controllers
{
    [Authorize]
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioTipoInmueble repositorioTipoInmueble;
        private readonly IRepositorioPropietario repositorioPropietario;
        private readonly IRepositorioImagen repositorioImagen;
        private readonly ILogger<InmuebleController> logger;

        public InmuebleController(
            IRepositorioInmueble repositorio,
            IRepositorioTipoInmueble repositorioTipoInmueble,
            IRepositorioPropietario repositorioPropietario,
            IRepositorioImagen repositorioImagen,
            ILogger<InmuebleController> logger)
        {
            this.repositorio = repositorio;
            this.repositorioTipoInmueble = repositorioTipoInmueble;
            this.repositorioPropietario = repositorioPropietario;
            this.repositorioImagen = repositorioImagen;
            this.logger = logger;
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

                var propietarios = repositorioPropietario.ObtenerLista(1, repositorioPropietario.ObtenerCantidad());
                ViewBag.PropietariosInforme = new SelectList(
                    propietarios.Select(p => new { p.IdPropietario, NombreCompleto = $"{p.Apellido}, {p.Nombre} - DNI: {p.Dni}" }),
                    "IdPropietario", "NombreCompleto");

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
                ViewBag.Imagenes = repositorioImagen.BuscarPorInmueble(inmueble.IdInmueble);

                return View(inmueble);
            }
            catch (Exception ex)
            {
                TempData["error"] = "No se pudo cargar el inmueble. Intente nuevamente." + ex.Message;
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
        public IActionResult Create(Inmueble inmueble, [FromServices] IWebHostEnvironment environment, IFormFile? archivoPortada)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos(inmueble);
                return View(inmueble);
            }

            try
            {
                inmueble.Estado = true;
                if (archivoPortada != null)
                {
                    inmueble.Portada = GuardarPortada(archivoPortada, environment);
                }

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
        public IActionResult Edit(int id, Inmueble inmueble, [FromServices] IWebHostEnvironment environment, IFormFile? archivoPortada)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos(inmueble);
                return View(inmueble);
            }

            try
            {
                inmueble.IdInmueble = id;
                var inmuebleActual = repositorio.ObtenerPorId(id);
                if (inmuebleActual == null)
                {
                    TempData["error"] = "El inmueble no existe.";
                    return RedirectToAction(nameof(Index));
                }
                if (archivoPortada != null)
                {
                    if (inmuebleActual.Portada is not null)
                    {
                        string rutaFisica = Path.Combine(environment.WebRootPath, inmuebleActual.Portada.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                    if (System.IO.File.Exists(rutaFisica))
                    {
                        System.IO.File.Delete(rutaFisica);
                    }
                    }
                    inmueble.Portada = GuardarPortada(archivoPortada, environment);
                }
                else
                {
                    inmueble.Portada = inmuebleActual?.Portada;
                }

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
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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
        // GET: Inmueble/BuscarDisponibles?fechaInicio=2026-09-10&fechaFin=2026-09-20
        [HttpGet]
        public IActionResult BuscarDisponibles(DateTime fechaIngreso, DateTime fechaEgreso, int? idReservaExcluir = null)
        {
            try
            {
                if (fechaEgreso <= fechaIngreso)
                {
                    return Json(new List<object>());
                }

                var disponibles = repositorio.ObtenerDisponiblesEntreFechas(fechaIngreso, fechaEgreso);

                var resultado = new List<object>();
                foreach (var i in disponibles)
                {
                     resultado.Add(new
                    {
                        id = i.IdInmueble,
                        texto = $"{i.Direccion} ({i.TipoInmueble?.Nombre}) - Cupo: {i.Cupo} - ${i.PrecioDia}/día",
                        precioDia = i.PrecioDia
                    });
                }

                return Json(resultado);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al buscar inmuebles disponibles");
                return Json(new List<object>());
            }
        }

        private string GuardarPortada(IFormFile archivo, [FromServices] IWebHostEnvironment environment)
        {
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads", "Inmuebles", "Portadas");
        
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        
            string fileName = "portada_" + Guid.NewGuid() + Path.GetExtension(archivo.FileName);
            string rutaFisicaCompleta = Path.Combine(path, fileName);
        
            using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
            {
                archivo.CopyTo(stream);
            }
        
            return Path.Combine("/Uploads/Inmuebles", fileName).Replace("\\", "/");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult FiltrarPorDisponibilidad(string disponible = "todos", int pagina = 1)
        {
            try
            {
                int tamPagina = 10;
                pagina = Math.Max(pagina, 1);

                bool? filtro = disponible switch
                {
                    "si" => true,
                    "no" => false,
                    _ => null
                };

                var lista = repositorio.ObtenerPorDisponibilidad(filtro, pagina, tamPagina);
                int totalRegistros = repositorio.ObtenerCantidadPorDisponibilidad(filtro);
                int totalPaginas = totalRegistros == 0
                    ? 1
                    : (totalRegistros % tamPagina == 0 ? totalRegistros / tamPagina : totalRegistros / tamPagina + 1);

                var datos = lista.Select(i => new
                {
                    id = i.IdInmueble,
                    direccion = i.Direccion,
                    propietario = $"{i.Propietario?.Nombre} {i.Propietario?.Apellido}",
                    cupo = i.Cupo,
                    precioDia = i.PrecioDia.ToString("C"),
                    porcentajeReserva = i.PorcentajeReserva,
                    disponible = i.Disponible
                });

                return Json(new { inmuebles = datos, paginaActual = pagina, totalPaginas });
            }
            catch (Exception)
            {
              return Json(new { inmuebles = new List<object>(), paginaActual = 1, totalPaginas = 1 });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult BuscarPorFechasAjax(DateTime fechaInicio, DateTime fechaFin, int pagina = 1)
        {
            try
            {
                int tamPagina = 10;
                pagina = Math.Max(pagina, 1);

                if (fechaFin <= fechaInicio)
                {
                    return Json(new { inmuebles = new List<object>(), paginaActual = 1, totalPaginas = 1 });
                }

                var lista = repositorio.ObtenerDisponiblesEntreFechasPaginado(fechaInicio, fechaFin, pagina, tamPagina);
                int totalRegistros = repositorio.ObtenerCantidadDisponiblesEntreFechas(fechaInicio, fechaFin);
                int totalPaginas = totalRegistros == 0
                    ? 1
                    : (totalRegistros % tamPagina == 0 ? totalRegistros / tamPagina : totalRegistros / tamPagina + 1);

                var datos = lista.Select(i => new
                {
                    id = i.IdInmueble,
                    direccion = i.Direccion,
                    propietario = $"{i.Propietario?.Nombre} {i.Propietario?.Apellido}",
                    cupo = i.Cupo,
                    precioDia = i.PrecioDia.ToString("C"),
                    porcentajeReserva = i.PorcentajeReserva,
                    disponible = i.Disponible
                });

                return Json(new { inmuebles = datos, paginaActual = pagina, totalPaginas });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al buscar inmuebles disponibles entre fechas");
                return Json(new { inmuebles = new List<object>(), paginaActual = 1, totalPaginas = 1 });
            }
        }

        // Informe: inmuebles más reservados en los últimos "dias" (default 365)
        [HttpGet]
        [AllowAnonymous]
        public IActionResult MasReservados(int dias = 365, int pagina = 1)
        {
            try
            {
                int tamPagina = 10;
                pagina = Math.Max(pagina, 1);
                dias = dias <= 0 ? 365 : dias;

                var lista = repositorio.ObtenerMasReservados(dias, pagina, tamPagina);
                int totalRegistros = repositorio.ObtenerCantidadPorDisponibilidad(null);
                int totalPaginas = totalRegistros == 0
                    ? 1
                    : (totalRegistros % tamPagina == 0 ? totalRegistros / tamPagina : totalRegistros / tamPagina + 1);

                var datos = lista.Select(i => new
                {
                    id = i.IdInmueble,
                    direccion = i.Direccion,
                    propietario = $"{i.Propietario?.Nombre} {i.Propietario?.Apellido}",
                    cupo = i.Cupo,
                    precioDia = i.PrecioDia.ToString("C"),
                    cantidadReservas = i.CantidadReservas
                });

                return Json(new { inmuebles = datos, paginaActual = pagina, totalPaginas });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener el informe de inmuebles más reservados");
                return Json(new { inmuebles = new List<object>(), paginaActual = 1, totalPaginas = 1 });
            }
        }

        // Informe: inmuebles sin reservas en los últimos "dias" (configurable, ej. 30/60)
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SinReservas(int dias = 30, int pagina = 1)
        {
            try
            {
                int tamPagina = 10;
                pagina = Math.Max(pagina, 1);
                dias = dias <= 0 ? 30 : dias;

                var lista = repositorio.ObtenerSinReservas(dias, pagina, tamPagina);
                int totalRegistros = repositorio.ObtenerCantidadSinReservas(dias);
                int totalPaginas = totalRegistros == 0
                    ? 1
                    : (totalRegistros % tamPagina == 0 ? totalRegistros / tamPagina : totalRegistros / tamPagina + 1);

                var datos = lista.Select(i => new
                {
                    id = i.IdInmueble,
                    direccion = i.Direccion,
                    propietario = $"{i.Propietario?.Nombre} {i.Propietario?.Apellido}",
                    cupo = i.Cupo,
                    precioDia = i.PrecioDia.ToString("C"),
                    disponible = i.Disponible
                });

                return Json(new { inmuebles = datos, paginaActual = pagina, totalPaginas });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener el informe de inmuebles sin reservas");
                return Json(new { inmuebles = new List<object>(), paginaActual = 1, totalPaginas = 1 });
            }
        }

        // Informe: inmuebles de un propietario específico
        [HttpGet]
        [AllowAnonymous]
        public IActionResult PorPropietario(int idPropietario, int pagina = 1)
        {
            try
            {
                int tamPagina = 10;
                pagina = Math.Max(pagina, 1);

                var lista = repositorio.ObtenerPorPropietario(idPropietario, pagina, tamPagina);
                int totalRegistros = repositorio.ObtenerCantidadPorPropietario(idPropietario);
                int totalPaginas = totalRegistros == 0
                    ? 1
                    : (totalRegistros % tamPagina == 0 ? totalRegistros / tamPagina : totalRegistros / tamPagina + 1);

                var datos = lista.Select(i => new
                {
                    id = i.IdInmueble,
                    direccion = i.Direccion,
                    cupo = i.Cupo,
                    precioDia = i.PrecioDia.ToString("C"),
                    porcentajeReserva = i.PorcentajeReserva,
                    disponible = i.Disponible
                });

                return Json(new { inmuebles = datos, paginaActual = pagina, totalPaginas });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener el informe de inmuebles por propietario");
                return Json(new { inmuebles = new List<object>(), paginaActual = 1, totalPaginas = 1 });
            }
        }
    }
}