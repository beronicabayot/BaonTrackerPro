using System;
using System.ComponentModel.DataAnnotations;

namespace BaonTrackerPro.Models
{
    public class SavingsGoal
    {
        public int Id { get; set; }

        public int? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal TargetAmount { get; set; }

        public decimal CurrentAmount { get; set; }

        public DateTime? Deadline { get; set; }

        public string? Notes { get; set; }  

        public bool IsDone { get; set; } = false;
    }
}