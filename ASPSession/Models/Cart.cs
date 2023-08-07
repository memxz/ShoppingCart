using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPSession.Models
{
    public class Cart  
    {
        public Cart()
        {
            CartId++;
        }
        [Required]
        public int CartId { get; set; }
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        [ForeignKey("customreId")]
        public int CustomerId { get; set; }
        [Required]
        public int ItemCount { get; set; }

    }
}
