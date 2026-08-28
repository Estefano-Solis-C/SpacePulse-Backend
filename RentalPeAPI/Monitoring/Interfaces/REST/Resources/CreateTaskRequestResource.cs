using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para la creación de una solicitud de tarea por parte de un Homeowner.
/// 
/// Regla de Negocio:
/// - Solo Homeowner puede usar este endpoint
/// - Status se fuerza automáticamente a "PENDING"
/// - Las fechas planificadas se ignoran (siempre null)
/// - El CreatedByUserId se extrae del JWT, no del cliente
/// </summary>
public class CreateTaskRequestResource
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

    public CreateTaskRequestResource() { }

    public CreateTaskRequestResource(
        long spaceId,
        string title,
        string description,
        string? photoUrl = null)
    {
        SpaceId = spaceId;
        Title = title;
        Description = description;
        PhotoUrl = photoUrl;
    }
}

