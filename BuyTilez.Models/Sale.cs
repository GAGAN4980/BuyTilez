using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuyTilez.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        public string CreatedByUserId { get; set; }

        [ForeignKey("CreatedByUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public DateTime SaleDate { get; set; }

        public DateTime ShippingDate { get; set; }

        [Required]
        public double TotalSaleAmount { get; set; }

        public string SaleStatus { get; set; }

        public DateTime PaymentDate { get; set; }

        public string TransactionId { get; set; }  // BrainTree Transaction ID

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Email { get; set; }
    }
}
