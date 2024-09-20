using System.ComponentModel.DataAnnotations;

namespace TechWiz.Models
{
    public class SendMailDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string MailTo { get; set; }
        [Required] 
        public string Phone { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
