using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para la creación de una tarea (WorkItem) vinculada a un espacio (Space).
/// 
/// Regla de Negocio: 
/// - Puede ser creada por el dueño del espacio (Homeowner) o por un remodelador (Remodeler).
/// - El estado inicial se asigna automáticamente según el rol:
///   • Homeowner: Status = "PENDING", las fechas planificadas son ignoradas
///   • Remodeler: Acepta Status y fechas del payload
/// 
/// Por seguridad, el CreatedByUserId se extrae del JWT, no del cliente.
/// </summary>
public class CreateWorkItemResource
{
    [Required(ErrorMessage = "SpaceId es requerido")]
    [JsonPropertyName("spaceId")]
    public long SpaceId { get; set; }

    [Required(ErrorMessage = "Title es requerido")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title debe tener entre 1 y 200 caracteres")]
    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [Required(ErrorMessage = "Description es requerido")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Description debe tener entre 1 y 500 caracteres")]
    [JsonPropertyName("description")]
    public string Description { get; set; } = default!;

    [StringLength(2048, ErrorMessage = "PhotoUrl no puede exceder 2048 caracteres")]
    [JsonPropertyName("photoUrl")]
    public string? PhotoUrl { get; set; } 

    [JsonPropertyName("plannedStartDate")]
    public DateTime? PlannedStartDate { get; set; } 

    [JsonPropertyName("plannedEndDate")]
    public DateTime? PlannedEndDate { get; set; } 

    public CreateWorkItemResource() { }

    public CreateWorkItemResource(
        long spaceId,
        string title,
        string description,
        string? photoUrl = null,
        DateTime? plannedStartDate = null,
        DateTime? plannedEndDate = null)
    {
        SpaceId = spaceId;
        Title = title;
        Description = description;
        PhotoUrl = photoUrl;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
    }
}