using System.ComponentModel.DataAnnotations;

namespace BaonTrackerPro.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = "";

        [MaxLength(64)]
        public string? Username { get; set; }

        public DateTime? DateOfBirthUtc { get; set; }

        [MaxLength(24)]
        public string? Gender { get; set; }

        [MaxLength(260)]
        public string? ProfileIconPath { get; set; }

        [Required]
        public string PasswordHash { get; set; } = "";

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}

