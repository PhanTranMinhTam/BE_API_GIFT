using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_PhanTranMinhTam_New.Data
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdOrder { get; set; }
        public int UserId { get; set; }
        public DateTime? Date { get; set; }
        public int Total { get; set; }
        public int EarnedPoints { get; set; }
        public User User { get; set; }
        public ICollection<Orderitem> Order_items { get; set; }
    }
}
