using System.ComponentModel.DataAnnotations;

namespace BuyTilez.Models
{
    public class ApplicationType
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The application type name is required")]
        public string Name { get; set; }
    }
}
