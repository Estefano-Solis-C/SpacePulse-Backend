using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Property.Interfaces.Rest.Resources;

/// <summary>
/// DTO para la creación de un espacio (Space/Obra) en el marketplace de remodelaciones.
/// Alineado estrictamente con el contrato JSON definitivo.
/// </summary>
public class CreateSpaceResource
{
    [Required(ErrorMessage = "HomeownerId es requerido")]
    [JsonPropertyName("homeownerId")]
    public Guid HomeownerId { get; set; }

    [Required(ErrorMessage = "Title es requerido")]
    [StringLength(100, ErrorMessage = "Title no puede exceder 100 caracteres")]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description es requerida")]
    [StringLength(500, ErrorMessage = "Description no puede exceder 500 caracteres")]
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Location es requerida")]
    [StringLength(200, ErrorMessage = "Location no puede exceder 200 caracteres")]
    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "SpaceType es requerido")]
    [JsonPropertyName("spaceType")]
    public int SpaceType { get; set; }

    [Required(ErrorMessage = "DimensionsSquareMeters es requerido")]
    [Range(0.01, float.MaxValue, ErrorMessage = "DimensionsSquareMeters debe ser mayor a 0")]
    [JsonPropertyName("dimensionsSquareMeters")]
    public decimal DimensionsSquareMeters { get; set; }

    [Required(ErrorMessage = "EstimatedBudget es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "EstimatedBudget debe ser mayor a 0")]
    [JsonPropertyName("estimatedBudget")]
    public decimal EstimatedBudget { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "PEN";

    [JsonPropertyName("hasIot")]
    public bool HasIot { get; set; } = false;

    [JsonPropertyName("images")]
    public List<string> Images { get; set; } = new();
}