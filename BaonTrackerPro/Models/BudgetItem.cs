using System.ComponentModel.DataAnnotations;

namespace BaonTrackerPro.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public decimal MonthlyAmount { get; set; }
    }
}