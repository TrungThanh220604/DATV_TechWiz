﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechWiz.Models
{
    public class User : IdentityUser
    {
        public string? link_avatar {  get; set; }
        [ForeignKey("Currency")]
        public Currency? currency { get; set; }

        public ICollection<Trip>? Trips { get; set; }
    }
}
