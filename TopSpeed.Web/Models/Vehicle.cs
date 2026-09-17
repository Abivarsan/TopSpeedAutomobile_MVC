using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopSpeed.Web.Models
{
    public class Vehicle
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vehicle model name is required.")]
        [StringLength(100, ErrorMessage = "Model name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Manufacturer Brand is required.")]
        [Display(Name = "Manufacturer Brand")]
        public Guid BrandId { get; set; }

        [ForeignKey("BrandId")]
        public virtual Brand? Brand { get; set; }

        [Required]
        [Display(Name = "Category")]
        [StringLength(50)]
        public string VehicleType { get; set; } = "Supercar";

        [Required]
        [Range(1900, 2035, ErrorMessage = "Please enter a valid model year.")]
        [Display(Name = "Model Year")]
        public int Year { get; set; }

        [Required]
        [Range(10, 3000, ErrorMessage = "Horsepower must be between 10 and 3000 HP.")]
        [Display(Name = "Horsepower (HP)")]
        public int Horsepower { get; set; }

        [Required]
        [Range(0.5, 30.0, ErrorMessage = "Acceleration must be between 0.5 and 30.0 seconds.")]
        [Display(Name = "0-100 km/h (sec)")]
        public double Acceleration { get; set; }

        [Required]
        [Range(50, 600, ErrorMessage = "Top speed must be between 50 and 600 km/h.")]
        [Display(Name = "Top Speed (km/h)")]
        public int TopSpeed { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(1000, 50000000, ErrorMessage = "Please enter a valid price.")]
        [Display(Name = "Base Price (USD)")]
        public decimal Price { get; set; }

        [Display(Name = "Engine / Powertrain")]
        [StringLength(150)]
        public string? Engine { get; set; }

        [Display(Name = "Transmission")]
        [StringLength(100)]
        public string? Transmission { get; set; }

        [Display(Name = "Vehicle Photo")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Overview & Specs")]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation collection for user bookmarks
        public virtual ICollection<UserGarageItem>? GarageBookmarks { get; set; }
    }
}
