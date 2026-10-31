
using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Options;

public class DonationOptions : IValidatableObject
{
    [Range(typeof(decimal), "1.00", "10000.00")]
    public decimal MinAmount { get; set; } = 1.00m;

    [Range(typeof(decimal), "1.00", "10000.00")]
    public decimal MaxAmount { get; set; } = 10000.00m;

    [Range(1, int.MaxValue)]
    public int PendingTimeoutMinutes { get; set; } = 30;

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string DefaultCurrency { get; set; } = "USD";

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (MaxAmount <= MinAmount)
        {
            yield return new ValidationResult(
                "MaxAmount must be greater than MinAmount.",
                new[] { nameof(MaxAmount), nameof(MinAmount) });
        }
    }
}
