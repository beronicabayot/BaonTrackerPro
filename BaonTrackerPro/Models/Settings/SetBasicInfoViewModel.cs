using System.ComponentModel.DataAnnotations;

namespace BaonTrackerPro.Models.Settings
{
    public class SetBasicInfoViewModel
    {
        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(24)]
        public string Gender { get; set; } = "";
    }
}

