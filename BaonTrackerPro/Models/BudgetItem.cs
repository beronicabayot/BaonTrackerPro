using System.ComponentModel.DataAnnotations;

namespace BaonTrackerPro.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public decimal AmountLimit { get; set; }

        [Required]
        public string Period { get; set; } = "Monthly";

        public DateTime BudgetMonth { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}