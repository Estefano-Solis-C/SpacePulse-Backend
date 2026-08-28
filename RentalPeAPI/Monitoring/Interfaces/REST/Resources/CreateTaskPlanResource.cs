using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para la creación de un plan de tarea por parte de un Remodeler.
/// 
/// Regla de Negocio:
/// - Solo Remodeler puede usar este endpoint
/// - Permite especificar Status, PlannedStartDate y PlannedEndDate
/// - El CreatedByUserId se extrae del JWT, no del cliente
/// - Status es requerido; las fechas son requeridas si se proporciona status técnico
/// </summary>
public class CreateTaskPlanResource
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

    [Required(ErrorMessage = "Status es requerido")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Status debe tener entre 1 y 50 caracteres")]
    [JsonPropertyName("status")]
    public string Status { get; set; } = default!;

    [JsonPropertyName("plannedStartDate")]
    public DateTime? PlannedStartDate { get; set; } 

    [JsonPropertyName("plannedEndDate")]
    public DateTime? PlannedEndDate { get; set; } 

    [Required(ErrorMessage = "El precio no puede ser negativo")]
    [JsonPropertyName("price")]
    public decimal Price { get; set; } = 0;

    public CreateTaskPlanResource() { }

    public CreateTaskPlanResource(
        long spaceId,
        string title,
        string description,
        string status,
        string? photoUrl = null,
        DateTime? plannedStartDate = null,
        DateTime? plannedEndDate = null,
        decimal price = 0)
    {
        SpaceId = spaceId;
        Title = title;
        Description = description;
        PhotoUrl = photoUrl;
        Status = status;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        Price = price;
    }
}

