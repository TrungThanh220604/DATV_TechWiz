using System.ComponentModel.DataAnnotations;

namespace TechWiz.Models
{
    public class Destination
    {
        [Key]
        public int Id { get; set; }
        public string? DesName { get; set; }
        public string? link_GPS { get; set; }
        public ICollection<Gallery>? Galleries { get; set; }
        public ICollection<Trip>? Trips { get; set; }
    }
}
