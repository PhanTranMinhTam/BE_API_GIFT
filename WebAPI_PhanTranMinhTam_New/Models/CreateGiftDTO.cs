namespace WebAPI_PhanTranMinhTam_New.Models
{
    public class CreateGiftDTO
    {
        public string Name { get; set; }
        public IFormFile? Image { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Type { get; set; }
        public int? Idparents { get; set; }
        public int Quantity { get; set; }
        public int QuantityRequired { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int RoleId { get; set; }
    }
}
