# 📚 API Documentation

## Project Management System API

Esta es la documentación de la API REST del Sistema de Gestión de Proyectos desarrollado con .NET 8.0.

### 🔐 Autenticación

La API utiliza JWT (JSON Web Tokens) para la autenticación. Todos los endpoints requieren un token válido excepto:
- `POST /api/auth/register` - Registro de usuarios
- `POST /api/auth/login` - Inicio de sesión

### 📋 Endpoints Principales

#### Autenticación
- `POST /api/auth/register` - Registrar nuevo usuario
- `POST /api/auth/login` - Iniciar sesión

#### Usuarios
- `GET /api/users` - Obtener lista de usuarios
- `GET /api/users/{id}` - Obtener usuario por ID
- `PUT /api/users/{id}` - Actualizar usuario
- `DELETE /api/users/{id}` - Eliminar usuario

#### Proyectos
- `GET /api/projects` - Obtener todos los proyectos
- `GET /api/projects/{id}` - Obtener proyecto por ID
- `POST /api/projects` - Crear nuevo proyecto
- `PUT /api/projects/{id}` - Actualizar proyecto
- `DELETE /api/projects/{id}` - Eliminar proyecto

#### Tareas
- `GET /api/tasks` - Obtener todas las tareas
- `GET /api/tasks/{id}` - Obtener tarea por ID
- `GET /api/tasks/project/{projectId}` - Obtener tareas por proyecto
- `POST /api/tasks` - Crear nueva tarea
- `PUT /api/tasks/{id}` - Actualizar tarea
- `DELETE /api/tasks/{id}` - Eliminar tarea

### 🗄️ Base de Datos

La aplicación utiliza Azure SQL Database con las siguientes tablas:
- **Users** - Información de usuarios
- **Projects** - Datos de proyectos
- **Tasks** - Tareas asociadas a proyectos
- **Comments** - Comentarios en tareas

### 🚀 Despliegue

La aplicación está desplegada en Azure con:
- **Azure App Service** - Hosting del backend
- **Azure SQL Database** - Base de datos
- **Azure Resource Group** - Agrupación de recursos

### 📊 Monitoreo

El proyecto incluye:
- **GitHub Actions** - CI/CD automático
- **SonarQube** - Análisis de calidad de código
- **Swagger/OpenAPI** - Documentación interactiva de la API

---

*Documentación generada automáticamente - $(date)*