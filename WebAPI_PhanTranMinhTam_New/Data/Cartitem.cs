using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_PhanTranMinhTam_New.Data
{
    public class Cartitem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCartItem { get; set; }
        public int IdCart { get; set; }
        public int IdGift { get; set; }
        public int QuantityGift { get; set; }
        public Cart Carts { get; set; }
        public Gift Gifts { get; set; }
    }
}
