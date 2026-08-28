// Monitoring/Interfaces/REST/Resources/UpdateTaskProgressResource.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para actualizar el progreso (estado y fechas) de una tarea.
/// 
/// Regla de Negocio:
/// - Solo el Remodeler asignado al Space puede usar este endpoint
/// - SOLO permite actualizar: Status, PlannedStartDate, PlannedEndDate
/// - Title, Description, PhotoUrl son ignorados si se envían
/// </summary>
public class UpdateTaskProgressResource
{
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Status debe tener entre 1 y 50 caracteres")]
    [JsonPropertyName("status")]
    public string? Status { get; set; } 

    [JsonPropertyName("plannedStartDate")]
    public DateTime? PlannedStartDate { get; set; } 

    [JsonPropertyName("plannedEndDate")]
    public DateTime? PlannedEndDate { get; set; } 

    [JsonPropertyName("price")]
    public decimal? Price { get; set; } 

    public UpdateTaskProgressResource() { }

    public UpdateTaskProgressResource(
        string? status = null,
        DateTime? plannedStartDate = null,
        DateTime? plannedEndDate = null,
        decimal? price = null)
    {
        Status = status;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        Price = price;
    }
}

