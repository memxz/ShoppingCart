using System.ComponentModel.DataAnnotations;

namespace ASPSession.Models
{
    public class Customers
    {
        public Customers()
        {
            CustomerId++;
        }
        [Key]
        public int CustomerId { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public virtual Cart Cart { get; set; }
    }
}
