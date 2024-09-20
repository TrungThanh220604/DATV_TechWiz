using System.ComponentModel.DataAnnotations;

namespace TechWiz.Models
{
    public class Currency
    {
        [Key]
        public int Id { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }
        public double? ExchangeRate { get; set; }
        public DateTime? updateTime { get; set; }
        public ICollection<User>? Users { get; set; }

    }
}
