using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuyTilez.Models
{
    public class Product
    {
        public Product()
        {
            TempSquareMeters = 1;
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Short description is required")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Product description is required")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Product price is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public double Price { get; set; }

        public string? ImageUrl { get; set; }

        // Foreign Key
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public int ApplicationTypeId { get; set; }

        [ForeignKey("ApplicationTypeId")]
        public virtual ApplicationType? ApplicationType { get; set; }

        [NotMapped] // This property will not be mapped to the database
        [Range(1, 10000)]
        public int TempSquareMeters { get; set; }
    }
}
