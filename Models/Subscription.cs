using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        public string SubscriberId { get; set; }
        public ApplicationUser Subscriber { get; set; }

        public string TargetUserId { get; set; }
        public ApplicationUser TargetUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
