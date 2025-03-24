using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_PhanTranMinhTam_New.Data
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCart { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Cartitem> Cart_items { get; set; }
    }
}
