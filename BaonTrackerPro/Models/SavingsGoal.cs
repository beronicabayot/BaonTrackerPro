using System;
using System.ComponentModel.DataAnnotations;

namespace BaonTrackerPro.Models
{
    public class SavingsGoal
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal TargetAmount { get; set; }

        public decimal CurrentAmount { get; set; }

        public DateTime? Deadline { get; set; }
    }
}