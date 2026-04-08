using System;
using System.ComponentModel.DataAnnotations;

namespace BaonTrackerPro.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        public string? Description { get; set; }

        public string? Notes { get; set; }

        // Added for dashboard
        public string CategoryIcon { get; set; } = "💰";
        public bool IsIncome { get; set; } = false;
    }
}