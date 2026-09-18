# Agencia Inmobiliaria

Sitio web desarrollado con **ASP.NET Core MVC** para la gestión de una inmobiliaria dedicada a 
alquileres temporarios: administración de propietarios, inquilinos, inmuebles y reservas.

---

## Integrantes del grupo

| Nombre completo |
|---|
| Rocamora, Marianela |
| Fernández, Rocío |

---

## Tecnologías utilizadas

| Categoría | Tecnología |
|---|---|
| Framework | ASP.NET Core MVC (.NET) |
| Base de datos | MySQL |
| Acceso a datos | ADO.NET (`MySql.Data`) |
| Patrón de acceso a datos | Repository |
| Mapas | Leaflet + OpenStreetMap |

---

## Herramientas de desarrollo

| Herramienta | Uso |
|---|---|
| [Visual Studio Code](https://code.visualstudio.com/) | Editor principal del proyecto |
| [.NET SDK](https://dotnet.microsoft.com/download) | Compilación y ejecución del proyecto ASP.NET Core |
| [XAMPP](https://www.apachefriends.org/) | Servidor local MySQL/MariaDB + phpMyAdmin |
| [GitHub Desktop](https://desktop.github.com/) | Control de versiones y colaboración en equipo |

---

## Paquetes NuGet utilizados

| Paquete | Función |
|---|---|
| `MySql.Data` | Conector ADO.NET para conectar la app con MySQL |

Para instalarlo manualmente:

```bash
dotnet add package MySql.Data
```

---

## Instrucciones para levantar la base de datos

### Requisitos previos

- Tener **XAMPP** instalado (o cualquier servidor MySQL/MariaDB local).
- Tener el módulo **MySQL** de XAMPP corriendo (puerto por defecto: `3306`).

### Pasos

1. Iniciar **MySQL** desde el Panel de Control de XAMPP.
2. Abrir **phpMyAdmin** desde [`http://localhost/phpmyadmin`](http://localhost/phpmyadmin).
3. Crear una base de datos nueva llamada **`inmobiliaria`**.
4. Entrar a la base `inmobiliaria` y abrir la pestaña **Importar**.
5. Seleccionar el archivo `inmobiliaria.sql` de este repositorio y hacer clic en **Continuar**.
6. Verificar en la pestaña **Estructura** que se hayan creado las tablas: `propietario`, `inquilino`, `tipo_inmueble`, `inmueble` y `reserva` , `imagen` , `pago` , `usuario`.

---

## Configuración de la cadena de conexión

En `appsettings.json`, verificar que la cadena de conexión apunte a la base local:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=inmobiliaria;Uid=root;Pwd=;"
}
```

> Esta es la configuración por defecto de XAMPP (usuario `root` sin contraseña).
> Si tu instalación de MySQL usa otro usuario o contraseña, ajustar estos valores.

---

## Cómo correr el proyecto

```bash
dotnet run
```

## Credenciales de acceso
Rol           |	Email	                  |Contraseña     |
Administrador	| admin@inmobiliaria.com  |	Admin2026!    | 
Empleado	    |empleado@inmobiliaria.com|	Empleado2026! |

Estos usuarios se crean mediante el INSERT incluido en inmobiliaria.sql.

## Diagrama Entidad-Relación

![Diagrama Entidad-Relación](docs/DiagramaCompleto.png)