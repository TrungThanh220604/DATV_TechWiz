using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TechWiz.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string? CateName { get; set; }
        public string? Note { get; set; }
        public double? Budget { get; set; }
        [ForeignKey("Trip")]
        public int trip_id { get; set; }
        public Trip trip { get; set; }
        public ICollection<Items>? Itemss { get; set; }

    }
}
