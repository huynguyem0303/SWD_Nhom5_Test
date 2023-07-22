namespace TableReservation.ViewModels
{
    public class ViewStaffReservation
    {
        public int ReservationId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName {get; set;}  
        public int RestaurantId { get; set; }
        public int TableId { get; set; }
        public DateTime Time { get; set; }
        public string? Note { get; set; }
        public string Status { get; set; }
        public int Size { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
