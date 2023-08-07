using System.ComponentModel.DataAnnotations;

namespace ASPSession.Models
{
    public class Product
    {
        public Product()
        {
            ProductId++;
        }
        [Required]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductDesc { get; set; }
        public string ProductIMG { get; set; }

    }
}
