using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TechWiz.Models
{
    public class Trip
    {
        [Key]
        public int Id { get; set; }
        public string? TripName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set;}
        public double? Budget { get; set; }
        [ForeignKey("Destination")]
        public int? des_id { get; set; }
        public Destination destination { get; set; }

        [ForeignKey("User")]
        public string user_id { get; set; }
        public User user { get; set; }

        public ICollection<Category>? Categories { get; set; }
        public ICollection<Items>? Itemss { get; set; }


    }
}
