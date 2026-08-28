using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para actualizar una tarea (WorkItem) existente.
/// 
/// **TODOS los campos son OPCIONALES.** La lógica de autorización decide qué se actualiza:
/// 
/// - **Si eres el CREADOR (Homeowner):**
///   ✓ Puedes editar: Title, Description, PhotoUrl
///   ✗ Status, PlannedStartDate, PlannedEndDate serán RECHAZADOS
/// 
/// - **Si eres el Remodeler del Space:**
///   ✓ Puedes editar: Status, PlannedStartDate, PlannedEndDate
///   ✗ Title, Description, PhotoUrl serán RECHAZADOS
/// 
/// - **Si no eres ni creador ni remodeler:** 403 Forbid
/// </summary>
public class UpdateWorkItemResource
{
    [StringLength(200, ErrorMessage = "Title no puede exceder 200 caracteres")]
    [JsonPropertyName("title")]
    public string? Title { get; set; } 

    [StringLength(500, ErrorMessage = "Description no puede exceder 500 caracteres")]
    [JsonPropertyName("description")]
    public string? Description { get; set; } 

    [StringLength(2048, ErrorMessage = "PhotoUrl no puede exceder 2048 caracteres")]
    [JsonPropertyName("photoUrl")]
    public string? PhotoUrl { get; set; } 

    [StringLength(50, ErrorMessage = "Status no puede exceder 50 caracteres")]
    [JsonPropertyName("status")]
    public string? Status { get; set; } 

    [JsonPropertyName("plannedStartDate")]
    public DateTime? PlannedStartDate { get; set; } 

    [JsonPropertyName("plannedEndDate")]
    public DateTime? PlannedEndDate { get; set; } 

    public UpdateWorkItemResource() { }

    public UpdateWorkItemResource(
        string? title = null,
        string? description = null,
        string? photoUrl = null,
        string? status = null,
        DateTime? plannedStartDate = null,
        DateTime? plannedEndDate = null)
    {
        Title = title;
        Description = description;
        PhotoUrl = photoUrl;
        Status = status;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
    }
}

