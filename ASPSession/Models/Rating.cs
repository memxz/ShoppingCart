namespace ASPSession.Models;

public class Rating
{
    public int RatingId { get; set; }
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
    public Double RatingValue { get; set; }

}
