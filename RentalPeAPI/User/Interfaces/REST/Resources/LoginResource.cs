using System.ComponentModel.DataAnnotations;

namespace RentalPeAPI.User.Interfaces.REST.Resources;

public record LoginResource(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    string Password
);