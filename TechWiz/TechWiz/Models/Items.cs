using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TechWiz.Models
{
    public class Items
    {
        [Key]
        public int Id { get; set; }
        public string? ItemName { get; set; }
        public string? Note { get; set; }
        public double? Budget { get; set; }
        public DateTime? Date { get; set; }
        [ForeignKey("Trip")]
        public int trip_id { get; set; }
        public Trip trip { get; set; }

        [ForeignKey("Category")]
        public int? cate_id { get; set; }
        public Category category { get; set; }
    }
}
