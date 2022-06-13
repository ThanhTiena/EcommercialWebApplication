using System.ComponentModel.DataAnnotations;

namespace EcommercialWebApplication.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        [MaxLength(20)]
        public string CategoryName { get; set; }

        public ICollection<Product> Products { get; set; }

    }
}
