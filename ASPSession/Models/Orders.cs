using System.ComponentModel.DataAnnotations;

namespace ASPSession.Models
{
    public class Orders
    {
        public Orders()
        {

        }
        [Key]
        public int OrderId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public int ProductQty { get; set; }


    }
}
