using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para actualizar el contenido (texto y foto) de una tarea.
/// 
/// Regla de Negocio:
/// - Solo el creador (Homeowner o Remodeler que creó la tarea) puede usar este endpoint
/// - SOLO permite actualizar: Title, Description, PhotoUrl
/// - Status y fechas planificadas son ignorados si se envían
/// </summary>
public class UpdateTaskContentResource
{
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title debe tener entre 1 y 200 caracteres")]
    [JsonPropertyName("title")]
    public string? Title { get; set; } 

    [StringLength(500, MinimumLength = 1, ErrorMessage = "Description debe tener entre 1 y 500 caracteres")]
    [JsonPropertyName("description")]
    public string? Description { get; set; } 

    [StringLength(2048, ErrorMessage = "PhotoUrl no puede exceder 2048 caracteres")]
    [JsonPropertyName("photoUrl")]
    public string? PhotoUrl { get; set; } 

    public UpdateTaskContentResource() { }

    public UpdateTaskContentResource(
        string? title = null,
        string? description = null,
        string? photoUrl = null)
    {
        Title = title;
        Description = description;
        PhotoUrl = photoUrl;
    }
}

