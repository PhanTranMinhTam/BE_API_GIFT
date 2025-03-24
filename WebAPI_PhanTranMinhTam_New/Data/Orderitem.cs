namespace WebAPI_PhanTranMinhTam_New.Data
{
    public class Orderitem
    {
        public int IdOrderItem { get; set; }
        public int IdOrder { get; set; }
        public int IdGift { get; set; }
        public int? IdParent { get; set; }
        public int QuantityGift { get; set; }
        public Orderitem ParentGift { get; set; }
        public ICollection<Orderitem> ChildGifts { get; set; }
        public Order Orders { get; set; }
        public Gift Gifts { get; set; }
    }
}
