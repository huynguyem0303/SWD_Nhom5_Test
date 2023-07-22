using System.ComponentModel.DataAnnotations.Schema;

namespace TableReservation.ViewModels
{
    public class ViewTables
    {
        public int TableId { get; set; }
        public int Size { get; set; }
        public int RestaurantId { get; set; }
        public bool Status { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}
