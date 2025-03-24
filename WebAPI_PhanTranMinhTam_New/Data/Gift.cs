using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebAPI_PhanTranMinhTam_New.Data
{
    public class Gift
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdGift { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Type { get; set; }
        public int? Idparents { get; set; }
        public int Quantity { get; set; }
        public int QuantityRequired { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int RoleId { get; set; }
        public Gift ParentGift { get; set; }
        public ICollection<Gift> ChildGifts { get; set; }
        public ICollection<Cartitem> Cartitems { get; set; }
        public ICollection<Orderitem> Orderitem { get; set; }
    }
}
