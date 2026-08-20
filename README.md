# Agencia_inmobiliaria
Sitio web desarrollado con ASP.NET Core MVC para la gestión de una inmobiliaria.
Primera entrega: ABM (Alta, Baja, Modificación) de Propietarios e Inquilinos.

Integrantes del grupo
Rocamora, Marianela
Fernández, Rocío
Tecnologías utilizadas
Framework: ASP.NET Core MVC (.NET)
Base de datos: MySQL
Acceso a datos: ADO.NET (MySql.Data)
Patrón de acceso a datos: Repository

Instrucciones para levantar la base de datos

Requisitos previos
Tener XAMPP instalado (o cualquier servidor MySQL/MariaDB local).
Tener el módulo MySQL de XAMPP corriendo (puerto por defecto: 3306).

Pasos
Iniciar MySQL desde el Panel de Control de XAMPP 
Abrir phpMyAdmin desde http://localhost/phpmyadmin.
Crear una base de datos nueva llamada inmobiliaria.
Entrar a la base inmobiliaria y abrir la pestaña Importar.
Seleccionar el archivo database.sql de este repositorio y hacer clic en Continuar.
Verificar en la pestaña Estructura que se hayan creado las tablas inquilino y propietario.
Configuración de la cadena de conexión

En appsettings.json, verificar que la cadena de conexión apunte a la base local:

json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=inmobiliaria;Uid=root;Pwd=;"
}

Configuración por defecto de XAMPP (usuario root sin contraseña). Si tu instalación de MySQL usa otro usuario o contraseña, ajustar estos valores.

Cómo correr el proyecto
dotnet run

Luego abrir en el navegador la URL que indique la consola y navegar a:

/Inquilinos — ABM de Inquilinos
/Propietarios — ABM de Propietarios
