using Donutella.Domain.Common.Base;
using Donutella.Domain.Entities;

public class Review : BaseEntity
{
    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int UserId { get; set; }

    public User User { get; set; } = null!;
}
