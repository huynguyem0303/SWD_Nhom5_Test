
using Microsoft.EntityFrameworkCore;
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
    [Keyless]
    public class Reviews
    {
        
        [ForeignKey("Customers")]
        public int CustomerId { get; set; }
        [ForeignKey("Restaurants")]
        public int RestaurantId { get; set; }
        [MaxLength(300)]
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual Restaurants? Restaurants { get; set; }
        public virtual Customers? Customers { get; set; }
    }
}
