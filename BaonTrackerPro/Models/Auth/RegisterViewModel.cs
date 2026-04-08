using System.ComponentModel.DataAnnotations;

namespace BaonTrackerPro.Models.Auth
{
    public class RegisterViewModel
    {
        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; } = "";
    }
}

