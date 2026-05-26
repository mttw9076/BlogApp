using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Friend
    {
        [Key]
        public int Id { get; set; }

        // Użytkownik, który wysłał zaproszenie
        [Required]
        public string RequesterId { get; set; }

        // Użytkownik, który otrzymał zaproszenie
        [Required]
        public string ReceiverId { get; set; }

        // Status relacji
        // Pending = oczekujące
        // Accepted = zaakceptowane
        // Rejected = odrzucone
        [Required]
        public FriendStatus Status { get; set; } = FriendStatus.Pending;

        // Data wysłania zaproszenia
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Data akceptacji / odrzucenia
        public DateTime? UpdatedAt { get; set; }
    }

    public enum FriendStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}
