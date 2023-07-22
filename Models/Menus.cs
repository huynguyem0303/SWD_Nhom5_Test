using Microsoft.EntityFrameworkCore.Migrations;
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
    public class Menus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MenuId { get; set; }
        [Required]
        [MaxLength(100)]
        public string? DishName { get; set; }
        [Required]
        [MaxLength(200)]
        public string? DishDetail { get; set; }
        public float Price { get; set; }
        [Required]
        [MaxLength(100)]
        public MenuType? Type { get; set; }
        [ForeignKey("Restaurants")]
        public int RestaurantId { get; set; }
        public string? ImgURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Restaurants? Restaurants { get; set; }
    } 
}
