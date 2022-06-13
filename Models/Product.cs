using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommercialWebApplication.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Display(Name = "Product Category")]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:####.#}")]
        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal Price { get; set; }

        [Display(Name = "In Stock")]
        public int Stock { get; set; }
        public string Image { get; set; }

        [Display(Name = "Product Status")]
        public ProductStatus Status { get; set; }
        [Display(Name= "Available")]
        public Boolean IsAvailable { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Add Date")]
        public DateTime DateCreated { get; set; }

        public Category Category { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }

    }
    public enum ProductStatus
    {
        IsComing, New, BestSaler
    }
}
