using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Agencia_inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace Agencia_inmobiliaria.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IRepositorioUsuario repositorio;
        private readonly ILogger<UsuarioController> logger;
        private readonly IWebHostEnvironment environment;


        public UsuarioController(IRepositorioUsuario repositorio, ILogger<UsuarioController> logger, IWebHostEnvironment environment)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.environment = environment;
        }

         [HttpGet]
         public IActionResult Login()
         {
             return View();
         }
     
         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> Login(string email, string password)
         {
             var usuario = repositorio.ObtenerPorEmail(email);
     
             if (usuario == null || !usuario.Estado)
             {
                 ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                 return View();
             }
     
             var hasher = new PasswordHasher<Usuario>();
             var resultado = hasher.VerifyHashedPassword(usuario, usuario.Password, password);
     
             if (resultado == PasswordVerificationResult.Failed)
             {
                 ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                 return View();
             }
     
             var claims = new List<Claim>
             {
                 new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                 new Claim(ClaimTypes.Name, usuario.Nombre),
                 new Claim(ClaimTypes.Surname, usuario.Apellido),
                 new Claim(ClaimTypes.Email, usuario.Email),
                 new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                 new Claim("Avatar", usuario.Avatar ?? "/Uploads/Avatares/avatar-default.png")
             };
     
             var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
             var principal = new ClaimsPrincipal(identity);
     
             await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
     
             return RedirectToAction("Index", "Home");
         }
     
         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> Logout()
         {
             await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
             return RedirectToAction(nameof(Login));
         }
     
         public IActionResult AccesoDenegado()
         {
             return View();
         }

          [HttpGet]
          [AllowAnonymous]
           public IActionResult Register()
           {
               return View();
           }
       
           [HttpPost]
           [AllowAnonymous]
           [ValidateAntiForgeryToken]
           public IActionResult Register(Usuario usuario, IFormFile? archivoAvatar)
           {
               if (string.IsNullOrWhiteSpace(usuario.Password))
               {
                   ModelState.AddModelError("Password", "La contraseña es obligatoria.");
               }
       
               if (!ModelState.IsValid)
               {
                   return View(usuario);
               }
       
               try
               {
                   if (repositorio.ObtenerPorEmail(usuario.Email) != null)
                   {
                       ModelState.AddModelError("Email", "Ya existe un usuario con ese email.");
                       return View(usuario);
                   }
       
                   var hasher = new PasswordHasher<Usuario>();
                   usuario.Password = hasher.HashPassword(usuario, usuario.Password!);
                   usuario.Rol = RolUsuario.Empleado;  
                   usuario.Estado = true;
                
                   if (archivoAvatar != null)
                    {
                        usuario.Avatar = GuardarAvatar(archivoAvatar);
                    }
                   repositorio.Alta(usuario);
                   TempData["success"] = "Usuario registrado exitosamente. Ya podés iniciar sesión.";
                   return RedirectToAction(nameof(Login));
               }
               catch (Exception ex)
               {
                   logger.LogError(ex, "Error al registrar usuario");
                   ModelState.AddModelError("", "No se pudo completar el registro. Intente nuevamente.");
                   return View(usuario);
               }
           }
            private string GuardarAvatar(IFormFile archivo)
            {
                string wwwPath = environment.WebRootPath;
                string path = Path.Combine(wwwPath, "Uploads", "Avatares");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Guid.NewGuid() + Path.GetExtension(archivo.FileName);
                string rutaFisicaCompleta = Path.Combine(path, fileName);

                using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                {
                    archivo.CopyTo(stream);
                }

                return Path.Combine("/Uploads/Avatares", fileName).Replace("\\", "/");
            }

             [Authorize(Roles = "Administrador")]
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
                    logger.LogError(ex, "Error al obtener el listado de usuarios");
                    TempData["error"] = "No se pudo cargar el listado. Intente nuevamente.";
                    ViewBag.PaginaActual = 1;
                    ViewBag.TotalPaginas = 1;
                    return View(new List<Usuario>());
                }
            }
        
            [Authorize(Roles = "Administrador")]
            public IActionResult Details(int id)
            {
                try
                {
                    var usuario = repositorio.ObtenerPorId(id);
                    if (usuario == null) return NotFound();
                    return View(usuario);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al obtener el detalle del usuario (Id: {Id})", id);
                    TempData["error"] = "No se pudo cargar el usuario. Intente nuevamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
        
            [Authorize(Roles = "Administrador")]
            public IActionResult Edit(int id)
            {
                try
                {
                    var usuario = repositorio.ObtenerPorId(id);
                    if (usuario == null) return NotFound();
                    return View(usuario);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al obtener el usuario para editar (Id: {Id})", id);
                    TempData["error"] = "No se pudo cargar el usuario. Intente nuevamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
        
            [HttpPost]
            [Authorize(Roles = "Administrador")]
            [ValidateAntiForgeryToken]
            public IActionResult Edit(int id, Usuario usuario)
            {
                if (id != usuario.IdUsuario) return NotFound();
        
                
                if (!ModelState.IsValid)
                {
                    return View(usuario);
                }
        
                try
                {
                    int filasAfectadas = repositorio.Modificacion(usuario);
        
                    if (filasAfectadas > 0)
                        TempData["success"] = "Usuario modificado exitosamente";
                    else
                        TempData["error"] = "No se pudo modificar el usuario.";
        
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al modificar el usuario (Id: {Id})", id);
                    ModelState.AddModelError("", "No se pudo guardar los cambios. Intente nuevamente.");
                    return View(usuario);
                }
            }
        
            [Authorize(Roles = "Administrador")]
            public IActionResult Delete(int id)
            {
                int idUsuarioActual = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if (id == idUsuarioActual)
                {
                    TempData["error"] = "No podés eliminar tu propio usuario.";
                    return RedirectToAction(nameof(Index));
                }
                try
                {
                    var usuario = repositorio.ObtenerPorId(id);
                    if (usuario == null) return NotFound();
                    return View(usuario);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al obtener el usuario para eliminar (Id: {Id})", id);
                    TempData["error"] = "No se pudo cargar el usuario. Intente nuevamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
        
            [HttpPost, ActionName("Delete")]
            [Authorize(Roles = "Administrador")]
            [ValidateAntiForgeryToken]
            public IActionResult DeleteConfirmed(int id)
            {
                try
                {
                    int filasAfectadas = repositorio.Baja(id);
        
                    if (filasAfectadas > 0)
                        TempData["success"] = "Usuario eliminado exitosamente";
                    else
                        TempData["error"] = "No se pudo eliminar el usuario.";
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al eliminar el usuario (Id: {Id})", id);
                    TempData["error"] = "No se pudo eliminar el usuario. Intente nuevamente.";
                }
        
                return RedirectToAction(nameof(Index));
            }

            [Authorize]
            public IActionResult MiPerfil()
            {
                int idUsuarioActual = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                try
                {
                    var usuario = repositorio.ObtenerPorId(idUsuarioActual);
                    if (usuario == null) return NotFound();
                    return View(usuario);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al obtener el perfil propio (Id: {Id})", idUsuarioActual);
                    TempData["error"] = "No se pudo cargar tu perfil. Intente nuevamente.";
                    return RedirectToAction("Index", "Home");
                }
            }

            [HttpPost]
            [Authorize]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> MiPerfil(Usuario usuarioForm, IFormFile? archivoAvatar, string? passwordActual, string? nuevaPassword)
            {
                int idUsuarioActual = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if (usuarioForm.IdUsuario != idUsuarioActual)
                {
                    return Forbid();
                }

                try
                {
                    var usuarioActual = repositorio.ObtenerPorId(idUsuarioActual);
                    if (usuarioActual == null) return NotFound();

                    usuarioActual.Nombre = usuarioForm.Nombre;
                    usuarioActual.Apellido = usuarioForm.Apellido;
                    usuarioActual.Telefono = usuarioForm.Telefono;
                    usuarioActual.Direccion = usuarioForm.Direccion;

                    bool quiereCambiarPassword = !string.IsNullOrWhiteSpace(passwordActual) || !string.IsNullOrWhiteSpace(nuevaPassword);

                    if (quiereCambiarPassword)
                    {
                        if (string.IsNullOrWhiteSpace(passwordActual) || string.IsNullOrWhiteSpace(nuevaPassword))
                        {
                            ModelState.AddModelError("", "Debe completar ambos campos de contraseña para cambiarla.");
                            return View(usuarioActual);
                        }

                        var hasher = new PasswordHasher<Usuario>();
                        var resultado = hasher.VerifyHashedPassword(usuarioActual, usuarioActual.Password, passwordActual);

                        if (resultado == PasswordVerificationResult.Failed)
                        {
                            ModelState.AddModelError("", "La contraseña actual es incorrecta.");
                            return View(usuarioActual);
                        }

                        if (nuevaPassword.Length < 6)
                        {
                            ModelState.AddModelError("", "La nueva contraseña debe tener al menos 6 caracteres.");
                            return View(usuarioActual);
                        }
  
                        usuarioActual.Password = hasher.HashPassword(usuarioActual, nuevaPassword);
                    }

                    if (archivoAvatar != null)
                    {
                        usuarioActual.Avatar = GuardarAvatar(archivoAvatar);
                    }

                    repositorio.Modificacion(usuarioActual);

                    
                    await RefrescarCookieAsync(usuarioActual);

                     TempData["success"] = quiereCambiarPassword 
                        ? "Perfil y contraseña actualizados exitosamente" 
                        : "Perfil actualizado exitosamente";
                    return RedirectToAction(nameof(MiPerfil));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al actualizar el perfil propio (Id: {Id})", idUsuarioActual);
                    TempData["error"] = "No se pudo actualizar tu perfil. Intente nuevamente.";
                    return RedirectToAction(nameof(MiPerfil));
                }
            }

            private async Task RefrescarCookieAsync(Usuario usuario)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Surname, usuario.Apellido),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                    new Claim("Avatar", usuario.Avatar ?? "/Uploads/Avatares/avatar-default.png")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }

    }
}