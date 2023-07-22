using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableReservation.Models.Enum;

namespace TableReservation.Models
{
    public class Reservations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReservationId { get; set; }
        [ForeignKey("Customers")]
        public int CustomerId { get; set; }
        [MaxLength(50)]
        public string? CustomerName { get; set; }
        public DateTime Time { get; set; }
        [ForeignKey("Restaurants")]
        public int RestaurantId { get; set; }
        public int TableId { get; set; }
        [MaxLength(300)]
        public string? Note { get; set; }
        [Required]
        public Status? Status { get; set; }
        public int Size { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual Customers? Customer { get; set; }
        public virtual Restaurants? Restaurants { get; set; }
    }
}
