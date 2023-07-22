using TableReservation.Models.Enum;

namespace TableReservation.ViewModels
{
    public class ViewMenus
    {
        public int MenuId { get; set; }
        public string? DishName { get; set; }
        public string? DishDetail { get; set; }
        public float Price { get; set; }
        public string? Type { get; set; }
        public string? ImgURL { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
