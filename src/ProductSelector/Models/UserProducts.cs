using System.ComponentModel.DataAnnotations.Schema;

namespace ProductSelector.Models
{
    public class UserProduct
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string Port { get; set; }
        public string? ConfigJson { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }
    }

}
