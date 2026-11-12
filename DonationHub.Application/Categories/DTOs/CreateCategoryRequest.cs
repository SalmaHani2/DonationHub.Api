
using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Categories.Categories;

public class CreateCategoryRequest
{
    [Required]
    [StringLength(100)]
    public string NameAr { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string NameEn { get; set; } = string.Empty;

    public string? DescriptionAr { get; set; }

    public string? DescriptionEn { get; set; }
}
