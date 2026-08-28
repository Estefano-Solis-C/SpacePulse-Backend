using System.ComponentModel.DataAnnotations;

namespace RentalPeAPI.User.Interfaces.REST.Resources;

public record RegisterUserResource(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    [MinLength(6)]
    string Password,

    [Required]
    string FullName,
    
    string? Phone = null,
    string Role = "Homeowner",
    string? Photo = null
);
