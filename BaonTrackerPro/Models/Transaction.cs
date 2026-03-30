using System;
using System.ComponentModel.DataAnnotations;

namespace BaonTrackerPro.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        public string? Description { get; set; }

        // New property for optional notes
        public string? Notes { get; set; }
    }
}