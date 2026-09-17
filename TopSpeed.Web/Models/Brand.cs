using System.ComponentModel.DataAnnotations;

namespace TopSpeed.Web.Models
{
    public class Brand
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Brand name is required.")]
        [StringLength(100, ErrorMessage = "Brand name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Display(Name = "Established Year")]
        public int EstablishedYear { get; set; }

        [Display(Name = "Brand Logo")]
        public string BrandLogo { get; set; }

        [Display(Name = "Country of Origin")]
        [StringLength(100)]
        public string? Country { get; set; }

        [Display(Name = "Founder")]
        [StringLength(150)]
        public string? Founder { get; set; }

        [Display(Name = "Headquarters")]
        [StringLength(150)]
        public string? Headquarters { get; set; }

        [Display(Name = "Official Website")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? Website { get; set; }

        [Display(Name = "Heritage Biography")]
        public string? Description { get; set; }

        // Navigation property for related vehicles
        public virtual ICollection<Vehicle>? Vehicles { get; set; }
    }
}
