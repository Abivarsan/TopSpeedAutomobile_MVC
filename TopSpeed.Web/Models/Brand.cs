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

        [Display(Name= "Established Year")]
        public int EstablishedYear { get; set; }

        [Display(Name = "Brand Logo")]
        public string  BrandLogo { get; set; }
    }
}
