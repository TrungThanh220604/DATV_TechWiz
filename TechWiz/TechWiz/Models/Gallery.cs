using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechWiz.Models
{
    public class Gallery
    {
        [Key]
        public int Id { get; set; }
        public string? link_pic { get; set; }
        [ForeignKey("Destination")]
        public Destination? destination { get; set; }
    }
}
