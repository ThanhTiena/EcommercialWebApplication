using System.ComponentModel.DataAnnotations;

namespace EcommercialWebApplication.Models
{
    public class Profile
    {
        [Key]
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullNamae
        {
            get
            {
                return FirstName +" "+LastName;
            }
        }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Of Birth")]
        public DateTime Dob { get; set; }

        public string Address { get; set; }
        public string Nationality { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }

        public Customer Customer { get; set; }
    }
    public enum Gender
    {
        Male, Female, Order
    }
}
