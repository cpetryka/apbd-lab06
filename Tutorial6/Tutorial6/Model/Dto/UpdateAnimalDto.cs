using System.ComponentModel.DataAnnotations;

namespace Tutorial6.Model.Dto;

public class UpdateAnimalDto
{
    [Required]
    [MinLength(4)]
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; }
    [Required]
    [MinLength(4)]
    [MaxLength(200)]
    public string Category { get; set; }
    [Required]
    [MinLength(4)]
    [MaxLength(200)]
    public string Area { get; set; }
}