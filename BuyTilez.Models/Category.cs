using System.ComponentModel.DataAnnotations;

namespace BuyTilez.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Order is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Order must be greater than zero")]
        public int DisplayOrder { get; set; }
    }
}