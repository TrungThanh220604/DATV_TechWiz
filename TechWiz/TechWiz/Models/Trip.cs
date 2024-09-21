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
        public Destination? destination { get; set; }

        [ForeignKey("User")]
        public User? user { get; set; }

        public ICollection<Category>? Categories { get; set; }
        public ICollection<Items>? Itemss { get; set; }


    }
}
