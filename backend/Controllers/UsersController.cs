using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Api.DTOs;
using ProjectManagement.Api.Services;
using System.Security.Claims;

namespace ProjectManagement.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de usuarios y autenticación
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        /// <summary>
        /// Constructor del controlador de usuarios
        /// </summary>
        /// <param name="userService">Servicio de usuarios</param>
        /// <param name="jwtService">Servicio JWT</param>
        public UsersController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        /// <param name="registerUserDto">Datos del usuario a registrar</param>
        /// <returns>Usuario registrado</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.RegisterUserAsync(registerUserDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Autentica un usuario
        /// </summary>
        /// <param name="loginDto">Datos de login</param>
        /// <returns>Token de autenticación</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.AuthenticateAsync(loginDto);
            
            if (user == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            var token = _jwtService.GenerateToken(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(60); // Configurar según JWT settings

            var response = new AuthResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                },
                ExpiresAt = expiresAt
            };

            return Ok(response);
        }

        /// <summary>
        /// Obtiene todos los usuarios (solo para administradores)
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Obtiene un usuario específico por ID
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario encontrado</returns>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            // Los usuarios solo pueden ver su propio perfil, excepto administradores
            var currentUserId = GetCurrentUserId();
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (currentUserId != id && currentUserRole != "Admin")
            {
                return Forbid("No tiene permisos para ver este usuario.");
            }

            var user = await _userService.GetUserByIdAsync(id);
            
            if (user == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado.");
            }

            return Ok(user);
        }

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="updateUserDto">Datos actualizados del usuario</param>
        /// <returns>Usuario actualizado</returns>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Los usuarios solo pueden actualizar su propio perfil, excepto administradores
            var currentUserId = GetCurrentUserId();
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (currentUserId != id && currentUserRole != "Admin")
            {
                return Forbid("No tiene permisos para actualizar este usuario.");
            }

            try
            {
                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                
                if (user == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado.");
                }

                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cambia la contraseña del usuario
        /// </summary>
        /// <param name="changePasswordDto">Datos del cambio de contraseña</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _userService.ChangePasswordAsync(userId, changePasswordDto);
            
            if (!result)
            {
                return BadRequest(new { message = "Contraseña actual incorrecta." });
            }

            return Ok(new { message = "Contraseña cambiada exitosamente." });
        }

        /// <summary>
        /// Elimina un usuario (solo para administradores)
        /// </summary>
        /// <param name="id">ID del usuario a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            
            if (!result)
            {
                return NotFound($"Usuario con ID {id} no encontrado.");
            }

            return NoContent();
        }

        /// <summary>
        /// Obtiene usuarios activos para asignación
        /// </summary>
        /// <returns>Lista de usuarios activos</returns>
        [HttpGet("active")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<UserSummaryDto>), 200)]
        public async Task<ActionResult<IEnumerable<UserSummaryDto>>> GetActiveUsers()
        {
            var users = await _userService.GetActiveUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Obtiene información del usuario actual
        /// </summary>
        /// <returns>Usuario actual</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), 200)]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            return Ok(user);
        }

        /// <summary>
        /// Obtiene el ID del usuario actual desde el token JWT
        /// </summary>
        /// <returns>ID del usuario</returns>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Token de usuario inválido.");
            }

            return userId;
        }
    }
}