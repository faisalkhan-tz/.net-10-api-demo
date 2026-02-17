using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record LoginDto(
    [Required] string Username,
    [Required] string Password
);
