using System.ComponentModel.DataAnnotations;

namespace BaonTrackerPro.Models.Settings
{
    public class UpdateUsernameViewModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(64)]
        [RegularExpression(@"^[a-zA-Z0-9._]+$", ErrorMessage = "Username can only contain letters, numbers, dots, and underscores.")]
        public string Username { get; set; } = "";
    }
}

