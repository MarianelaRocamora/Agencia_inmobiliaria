using Agencia_inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;

namespace Agencia_inmobiliaria.Controllers
{
    public class ImagenController : Controller
    {
        private readonly IRepositorioImagen repositorio;

		public ImagenController(IRepositorioImagen repositorio)
		{
			this.repositorio = repositorio;
		}

        [HttpPost]
		public async Task<IActionResult> Alta(int id, List<IFormFile> imagenes, [FromServices] IWebHostEnvironment environment)
		{
			if (imagenes == null || imagenes.Count == 0){
				//return BadRequest("No se recibieron archivos.");
				TempData["error"] = "Debe seleccionar al menos una imagen.";
        		return RedirectToAction("Details", "Inmueble", new { id = id });
			}
			string wwwPath = environment.WebRootPath;
			string path = Path.Combine(wwwPath, "Uploads");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			path = Path.Combine(path, "Inmuebles");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			path = Path.Combine(path, id.ToString());
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			foreach (var file in imagenes)
			{
				if (file.Length > 0)
				{
					var extension = Path.GetExtension(file.FileName);
					var nombreArchivo = $"{Guid.NewGuid()}{extension}";
					var rutaArchivo = Path.Combine(path, nombreArchivo);

					using (var stream = new FileStream(rutaArchivo, FileMode.Create))
					{
						await file.CopyToAsync(stream);
					}
					Imagen imagen = new Imagen
					{
						IdInmueble= id,
						Url = $"/Uploads/Inmuebles/{id}/{nombreArchivo}",
					};
					repositorio.Alta(imagen);
				}
			}
			TempData["success"] = "Imágenes agregadas exitosamente";
			return RedirectToAction("Details", "Inmueble", new { id = id});
		}

        // POST: Inmueble/Eliminar/:id
		[HttpPost]
		public ActionResult Eliminar(int idInmueble)
		{
			try
			{
				var imagen = repositorio.ObtenerPorId(idInmueble);
				if(imagen is null)
				{
					return NotFound();
				}
				int filasAfectadas = repositorio.Baja(idInmueble);
				if (filasAfectadas > 0)
        		{
        		    TempData["success"] = "Imagen eliminada exitosamente";
        		}
        		else
        		{
        		    TempData["error"] = "No se pudo eliminar la imagen.";
        		}
				//return Ok(repositorio.BuscarPorInmueble(imagen.IdInmueble));
			}
			catch (Exception ex)
			{
				//return BadRequest(ex.Message);
				TempData["error"] = "No se pudo eliminar la imagen. Intente nuevamente.";
			}
			return RedirectToAction("Details", "Inmueble", new { id = idInmueble });
		}
    }
}