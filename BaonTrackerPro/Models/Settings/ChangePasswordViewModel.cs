using System.ComponentModel.DataAnnotations;

namespace BaonTrackerPro.Models.Settings
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string CurrentPassword { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string NewPassword { get; set; } = "";
    }
}

