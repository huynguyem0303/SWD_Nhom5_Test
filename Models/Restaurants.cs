
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
    public class Restaurants
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RestaurantId { get; set; }
        [Required]
        [MaxLength(100)]
        public string RestaurantName { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [MaxLength(10)]
        public string? RestaurantPhone { get; set; }
        [Required]
        [MaxLength(100)]
        public string? RestaurantAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string OpenHours { get; set; }
        public bool Status { get; set; }
        //public ICollection<Reviews> Reviews { get; set; }
        //public ICollection<Menus> Menus { get; set; }
        //public ICollection<Tables> Tables { get; set; }

    }
}
