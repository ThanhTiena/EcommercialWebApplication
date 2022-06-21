using System.ComponentModel.DataAnnotations;

namespace EcommercialWebApplication.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        public int Count { get; set; }
        public decimal Discount { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
        public int UserId { get; set; }
        public CouponType? Type { get; set; }
        public ICollection<Order> Orders { get; set; }
    }

    public enum CouponType
    {
        General,
        Specific,
        Special
    }
}
