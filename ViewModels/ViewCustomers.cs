using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TableReservation.ViewModels
{
    public class ViewCustomers
    {
        public int CustomerId { get; set; }
        public string? Phone { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}
