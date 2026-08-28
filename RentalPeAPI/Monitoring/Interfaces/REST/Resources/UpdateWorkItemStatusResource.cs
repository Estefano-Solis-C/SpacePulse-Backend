using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para actualizar el estado de una tarea (WorkItem).
/// Por seguridad, NO incluye el ID del usuario (se extrae del JWT).
/// Solo solicita el nuevo estado.
/// Validación: solo el remodelador asignado al espacio puede cambiar el estado.
/// </summary>
public class UpdateWorkItemStatusResource
{
    [Required(ErrorMessage = "Status es requerido")]
    [StringLength(50, ErrorMessage = "Status no puede exceder 50 caracteres")]
    [JsonPropertyName("status")]
    public string Status { get; set; } = default!;

    public UpdateWorkItemStatusResource() { }

    public UpdateWorkItemStatusResource(string status)
    {
        Status = status;
    }
}

