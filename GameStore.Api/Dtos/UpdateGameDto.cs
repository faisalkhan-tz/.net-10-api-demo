using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class UpdateGameDto(
    int Id,
    [Required][StringLength(50)] string Name,
    [Range(1, int.MaxValue)] int GenreId,
    [Range(0, 100)] decimal Price,
    [Required] DateOnly ReleaseDate
);
