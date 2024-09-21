using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechWiz.Models
{
    public class AddTripViewModel
    {
        [Required(ErrorMessage = "Start date is required.")]
        public DateTime? StartTime { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime? EndTime { get; set; }

        [Required(ErrorMessage = "Budget is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Budget must be greater than 0.")]
        public double? Budget { get; set; }

        [Required(ErrorMessage = "Destination is required.")]
        public int DestinationId { get; set; } 

    }
}
