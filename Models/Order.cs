using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommercialWebApplication.Models
{
    public class Order
    {
        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public int UserId { get; set; }
        public int? CouponId { get; set; }

        [Display(Name = "Order Status")]
        public OrderStatus Status { get; set; }

        [Display(Name = "Receiver")]
        public string ShipName { get; set; }

        [Display(Name = "Address")]
        public string ShipAddress { get; set; }

        [Display(Name = "Phone")]
        public string ShipPhoneNumber { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string ShipEmail { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
        public Coupon Coupon { get; set; }

    }
    public enum OrderStatus
    {
        InProgress,
        Confirmed,
        Shipping,
        Success,
        Canceled
    }
    public enum PaymentMethod
    {
        [Display(Name ="COD")]
        Cash, 
        [Display(Name ="Online Banking")]
        OnlineBanking,
        [Display(Name = "Zalo Payment")]
        MobilePayments
    }
}
