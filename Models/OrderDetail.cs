using System.ComponentModel.DataAnnotations;

namespace EcommercialWebApplication.Models
{
    public class OrderDetail
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "Product Quantities")]
        public int quantity { get; set; }

        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
