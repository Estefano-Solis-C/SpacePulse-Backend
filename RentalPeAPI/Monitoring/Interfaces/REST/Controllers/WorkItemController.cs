using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;
using RentalPeAPI.Property.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Controllers;

/// <summary>
/// Controlador para la gestión de tareas (WorkItems/Tasks) vinculadas a espacios (Spaces).
/// Implementa una API basada en intención con endpoints separados por rol.
/// 
/// Arquitectura DDD con Endpoints Intent-Driven:
/// - POST /request: Homeowner crea solicitud de tarea (Status="PENDING")
/// - POST /plan: Remodeler crea plan de tarea (con Status, fechas)
/// - PUT /{id}/content: Creador edita contenido (título, descripción, foto)
/// - PUT /{id}/progress: Remodeler actualiza progreso (estado, fechas)
/// - DELETE /{id}: Solo creador puede eliminar
/// - GET /{id}: Obtener detalles de la tarea
/// </summary>
[ApiController]
[Route("api/v1/monitoring/tasks")] // -> /api/v1/monitoring/tasks
[Tags("Tasks")]
[Authorize]
public class WorkItemController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWorkItemRepository _workItemRepository;
    private readonly ISpaceRepository _spaceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WorkItemController(
        IMediator mediator,
        IWorkItemRepository workItemRepository,
        ISpaceRepository spaceRepository,
        IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _workItemRepository = workItemRepository;
        _spaceRepository = spaceRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Extrae el ID del usuario del token JWT.
    /// Devuelve Guid.Empty si el token es inválido.
    /// </summary>
    private Guid ExtractUserIdFromToken()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Guid.Empty;
        return userId;
    }

    /// <summary>
    /// Obtiene el rol del usuario del token JWT.
    /// Devuelve null si no hay rol.
    /// </summary>
    private string? GetUserRoleFromToken()
    {
        return User.FindFirstValue(ClaimTypes.Role);
    }

    /// <summary>
    /// [INTENT-DRIVEN] Crea una solicitud de tarea (Homeowner).
    /// 
    /// Reglas de Negocio Estrictas:
    /// - Solo usuarios con rol "Homeowner" pueden contactar este endpoint
    /// - Status se fuerza automáticamente a "PENDING"
    /// - Las fechas planificadas se ignoran (siempre null)
    /// - El CreatedByUserId se extrae del JWT y NO puede ser sobrescrito por el cliente
    /// 
    /// Devuelve 201 Created con el WorkItemResource completo.
    /// </summary>
    /// <param name="resource">DTO CreateTaskRequestResource con spaceId, title, description, photoUrl</param>
    /// <returns>201 Created con WorkItemResource completo</returns>
    /// <response code="201">Tarea creada exitosamente</response>
    /// <response code="400">Validación fallida o datos inválidos</response>
    /// <response code="401">Token JWT inválido, ausente o usuario no autenticado</response>
    /// <response code="403">Usuario no tiene rol "Homeowner"</response>
    [HttpPost("request")]
    [Authorize(Roles = "Homeowner")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> CreateTaskRequest([FromBody] CreateTaskRequestResource resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var createdByUserId = ExtractUserIdFromToken();
        if (createdByUserId == Guid.Empty)
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {
            var command = new CreateWorkItemCommand(
                resource.SpaceId,
                createdByUserId,
                resource.Title,
                resource.Description,
                resource.PhotoUrl,
                null,
                null,
                0m 
            );

            var taskId = await _mediator.Send(command);


            var createdWorkItem = await _workItemRepository.FindByIdAsync(taskId);
            if (createdWorkItem == null)
                throw new KeyNotFoundException($"WorkItem recién creado con ID {taskId} no encontrado.");

            var workItemResource = new WorkItemResource(
                createdWorkItem.Id,
                createdWorkItem.SpaceId,
                createdWorkItem.CreatedByUserId,
                createdWorkItem.Title,
                createdWorkItem.Description,
                createdWorkItem.PhotoUrl,
                createdWorkItem.PlannedStartDate,
                createdWorkItem.PlannedEndDate,
                createdWorkItem.Price,
                createdWorkItem.Status,
                createdWorkItem.CreatedAt,
                createdWorkItem.CompletedAt
            );

            return CreatedAtAction(
                nameof(GetWorkItemById),
                new { id = taskId },
                workItemResource
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Error al crear la solicitud de tarea: {ex.Message}" });
        }
    }

    /// <summary>
    /// [INTENT-DRIVEN] Crea un plan de tarea (Remodeler).
    /// 
    /// Reglas de Negocio Estrictas:
    /// - Solo usuarios con rol "Remodeler" pueden contactar este endpoint
    /// - Permite especificar Status, PlannedStartDate y PlannedEndDate
    /// - El CreatedByUserId se extrae del JWT y NO puede ser sobrescrito por el cliente
    /// - Las fechas se validan en el agregado de dominio (start &lt; end)
    /// 
    /// Devuelve 201 Created con el WorkItemResource completo.
    /// </summary>
    /// <param name="resource">DTO CreateTaskPlanResource con spaceId, title, description, photoUrl, status, fechas</param>
    /// <returns>201 Created con WorkItemResource completo</returns>
    /// <response code="201">Plan de tarea creado exitosamente</response>
    /// <response code="400">Validación fallida, datos inválidos o fechas inconsistentes</response>
    /// <response code="401">Token JWT inválido, ausente o usuario no autenticado</response>
    /// <response code="403">Usuario no tiene rol "Remodeler"</response>
    [HttpPost("plan")]
    [Authorize(Roles = "Remodeler")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> CreateTaskPlan([FromBody] CreateTaskPlanResource resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var createdByUserId = ExtractUserIdFromToken();
        if (createdByUserId == Guid.Empty)
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {
            var space = await _spaceRepository.FindByIdAsync(resource.SpaceId);
            if (space == null)
                return NotFound(new { error = $"Space con ID {resource.SpaceId} no encontrado." });

            if (space.RemodelerId != createdByUserId)
                return Forbid();
            
            var command = new CreateWorkItemCommand(
                resource.SpaceId,
                createdByUserId,
                resource.Title,
                resource.Description,
                resource.PhotoUrl,
                resource.PlannedStartDate,
                resource.PlannedEndDate,
                resource.Price
            );

            var taskId = await _mediator.Send(command);

            var createdWorkItem = await _workItemRepository.FindByIdAsync(taskId);
            if (createdWorkItem == null)
                throw new KeyNotFoundException($"WorkItem recién creado con ID {taskId} no encontrado.");

            var workItemResource = new WorkItemResource(
                createdWorkItem.Id,
                createdWorkItem.SpaceId,
                createdWorkItem.CreatedByUserId,
                createdWorkItem.Title,
                createdWorkItem.Description,
                createdWorkItem.PhotoUrl,
                createdWorkItem.PlannedStartDate,
                createdWorkItem.PlannedEndDate,
                createdWorkItem.Price,
                createdWorkItem.Status,
                createdWorkItem.CreatedAt,
                createdWorkItem.CompletedAt
            );

            return CreatedAtAction(
                nameof(GetWorkItemById),
                new { id = taskId },
                workItemResource
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Error al crear el plan de tarea: {ex.Message}" });
        }
    }

    /// <summary>
    /// Obtiene los detalles completos de una tarea específica.
    /// Requiere autenticación válida.
    /// </summary>
    /// <param name="id">ID de la tarea a obtener</param>
    /// <returns>200 OK con WorkItemResource si existe</returns>
    /// <response code="200">Tarea encontrada</response>
    /// <response code="401">Token JWT inválido o ausente</response>
    /// <response code="404">Tarea no encontrada</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetWorkItemById(int id)
    {
        try
        {
            var query = new GetWorkItemByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { error = $"WorkItem con ID {id} no encontrado." });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Error al obtener la tarea: {ex.Message}" });
        }
    }

    /// <summary>
    /// Obtiene todas las tareas asociadas a un espacio específico.
    /// Requiere autenticación válida.
    /// </summary>
    /// <param name="spaceId">ID del espacio</param>
    /// <returns>200 OK con lista de WorkItemResource</returns>
    /// <response code="200">Lista de tareas del espacio</response>
    /// <response code="401">Token JWT inválido o ausente</response>
    [HttpGet("space/{spaceId:long}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetWorkItemsBySpaceId(long spaceId)
    {
        try
        {
            var query = new GetWorkItemsBySpaceIdQuery(spaceId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Error al obtener tareas del espacio: {ex.Message}" });
        }
    }

    /// <summary>
    /// Obtiene todas las tareas asociadas al usuario autenticado.
    /// Retorna tareas en espacios donde el usuario es propietario (Homeowner) o remodelador asignado (Remodeler).
    /// 
    /// GET /api/v1/monitoring/tasks/my-tasks
    /// </summary>
    /// <returns>200 OK con lista de WorkItemResource</returns>
    /// <response code="200">Lista de tareas del usuario</response>
    /// <response code="401">Token JWT inválido o no autorizado</response>
    [HttpGet("my-tasks")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetMyTasks()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {
            var query = new GetWorkItemsByUserIdQuery(userId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Error al obtener las tareas del usuario: {ex.Message}" });
        }
    }

    /// <summary>
    /// [INTENT-DRIVEN] Actualiza el contenido (texto y foto) de una tarea.
    /// 
    /// Reglas de Autorización ESTRICTAS:
    /// - Solo el CREADOR EXACTO (User ID == CreatedByUserId) puede usar este endpoint
    /// - SOLO permite editar: Title, Description, PhotoUrl
    /// - Fields: Status, PlannedStartDate, PlannedEndDate son IGNORADOS si se envían
    /// - Si no es el creador: 403 Forbid
    /// 
    /// El usuario se extrae del token JWT.
    /// Devuelve 200 OK con el WorkItemResource actualizado.
    /// </summary>
    /// <param name="id">ID de la tarea a actualizar</param>
    /// <param name="resource">DTO UpdateTaskContentResource con title, description, photoUrl</param>
    /// <returns>200 OK con WorkItemResource actualizado</returns>
    /// <response code="200">Contenido de la tarea actualizado exitosamente</response>
    /// <response code="400">Validación fallida o contenido inválido</response>
    /// <response code="401">Token JWT inválido o ausente</response>
    /// <response code="403">Usuario no es el creador de la tarea</response>
    /// <response code="404">Tarea no encontrada</response>
    [HttpPut("{id:int}/content")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTaskContent(int id, [FromBody] UpdateTaskContentResource resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var requestingUserId = ExtractUserIdFromToken();
        if (requestingUserId == Guid.Empty)
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {
            var workItem = await _workItemRepository.FindByIdAsync(id);
            if (workItem == null)
                return NotFound(new { error = $"WorkItem con ID {id} no encontrado." });
            
            if (workItem.CreatedByUserId != requestingUserId)
                return Forbid();
            if (!string.IsNullOrWhiteSpace(resource.Title) ||
                !string.IsNullOrWhiteSpace(resource.Description) ||
                resource.PhotoUrl != null)
            {
                string finalTitle = !string.IsNullOrWhiteSpace(resource.Title) ? resource.Title : workItem.Title;
                string finalDescription = !string.IsNullOrWhiteSpace(resource.Description) ? resource.Description : workItem.Description;
                string? finalPhotoUrl = resource.PhotoUrl ?? workItem.PhotoUrl;

                workItem.EditContent(finalTitle, finalDescription, finalPhotoUrl);
                await _unitOfWork.CompleteAsync();
            }
            
             var workItemResource = new WorkItemResource(
                 workItem.Id,
                 workItem.SpaceId,
                 workItem.CreatedByUserId,
                 workItem.Title,
                 workItem.Description,
                 workItem.PhotoUrl,
                 workItem.PlannedStartDate,
                 workItem.PlannedEndDate,
                 workItem.Price,
                 workItem.Status,
                 workItem.CreatedAt,
                 workItem.CompletedAt
             );

             return Ok(workItemResource);
         }
         catch (ArgumentException ex)
         {
             return BadRequest(new { error = ex.Message });
         }
         catch (Exception ex)
         {
             return BadRequest(new { error = $"Error al actualizar el contenido: {ex.Message}" });
         }
     }

     /// <summary>
     /// [INTENT-DRIVEN] Actualiza el progreso (estado y fechas) de una tarea.
    /// 
    /// Reglas de Autorización ESTRICTAS:
    /// - Solo el Remodeler asignado al Space (User ID == Space.RemodelerId) puede usar este endpoint
    /// - SOLO permite editar: Status, PlannedStartDate, PlannedEndDate
    /// - Fields: Title, Description, PhotoUrl son IGNORADOS si se envían
    /// - Si no es el Remodeler asignado: 403 Forbid
    /// 
    /// El usuario se extrae del token JWT.
    /// Devuelve 200 OK con el WorkItemResource actualizado (incluyendo CompletedAt calculado).
    /// </summary>
    /// <param name="id">ID de la tarea a actualizar</param>
    /// <param name="resource">DTO UpdateTaskProgressResource con status, plannedStartDate, plannedEndDate</param>
    /// <returns>200 OK con WorkItemResource actualizado</returns>
    /// <response code="200">Progreso de la tarea actualizado exitosamente</response>
    /// <response code="400">Validación fallida o fechas inconsistentes</response>
    /// <response code="401">Token JWT inválido o ausente</response>
    /// <response code="403">Usuario no es el Remodeler asignado al Space</response>
    /// <response code="404">Tarea o Space no encontrado</response>
    [HttpPut("{id:int}/progress")]
    [Authorize(Roles = "Remodeler")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTaskProgress(int id, [FromBody] UpdateTaskProgressResource resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var requestingUserId = ExtractUserIdFromToken();
        if (requestingUserId == Guid.Empty)
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {
            var workItem = await _workItemRepository.FindByIdAsync(id);
            if (workItem == null)
                return NotFound(new { error = $"WorkItem con ID {id} no encontrado." });
            
            var space = await _spaceRepository.FindByIdAsync(workItem.SpaceId);
            if (space == null)
                return NotFound(new { error = $"Space con ID {workItem.SpaceId} no encontrado." });
            
            if (space.RemodelerId != requestingUserId)
                return Forbid();
            
            var finalStatus = !string.IsNullOrWhiteSpace(resource.Status) ? resource.Status : workItem.Status;
            
            var command = new UpdateWorkItemStatusCommand(id, finalStatus, requestingUserId, resource.Price);
            var updatedWorkItem = await _mediator.Send(command);

            if (updatedWorkItem == null)
                return NotFound(new { error = $"No se pudo actualizar la tarea {id}." });
            
            if (resource.PlannedStartDate.HasValue || resource.PlannedEndDate.HasValue)
            {
                DateTime? finalStartDate = resource.PlannedStartDate ?? updatedWorkItem.PlannedStartDate;
                DateTime? finalEndDate = resource.PlannedEndDate ?? updatedWorkItem.PlannedEndDate;
                
                updatedWorkItem.UpdateProgress(updatedWorkItem.Status, finalStartDate, finalEndDate);
                await _unitOfWork.CompleteAsync();
            }
            
             var workItemResource = new WorkItemResource(
                 updatedWorkItem.Id,
                 updatedWorkItem.SpaceId,
                 updatedWorkItem.CreatedByUserId,
                 updatedWorkItem.Title,
                 updatedWorkItem.Description,
                 updatedWorkItem.PhotoUrl,
                 updatedWorkItem.PlannedStartDate,
                 updatedWorkItem.PlannedEndDate,
                 updatedWorkItem.Price,
                 updatedWorkItem.Status,
                 updatedWorkItem.CreatedAt,
                 updatedWorkItem.CompletedAt
             );

             return Ok(workItemResource);
         }
         catch (ArgumentException ex)
         {
             return BadRequest(new { error = ex.Message });
         }
         catch (Exception ex)
         {
             return BadRequest(new { error = $"Error al actualizar el progreso: {ex.Message}" });
         }
     }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteWorkItem(int id)
    {
        var requestingUserId = ExtractUserIdFromToken();
        if (requestingUserId == Guid.Empty)
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {
            var workItem = await _workItemRepository.FindByIdAsync(id);
            if (workItem == null)
                return NotFound(new { error = $"WorkItem con ID {id} no encontrado." });
            
            if (workItem.CreatedByUserId != requestingUserId)
                return Forbid();
            
            await _workItemRepository.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Error al eliminar la tarea: {ex.Message}" });
        }
    }

}

