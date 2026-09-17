using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopSpeed.Web.Models
{
    public class Inquiry
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Your name is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string? Phone { get; set; }

        public Guid? VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public virtual Vehicle? Vehicle { get; set; }

        [Required(ErrorMessage = "Preferred test drive date is required.")]
        [Display(Name = "Preferred Test Drive Date")]
        public DateTime PreferredDate { get; set; } = DateTime.Today.AddDays(2);

        [StringLength(1000)]
        public string? Message { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
