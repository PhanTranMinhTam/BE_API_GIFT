namespace WebAPI_PhanTranMinhTam_New.Models
{
    public class UpdateUserDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? BirthDate { get; set; }
        public IFormFile Image { get; set; }
    }
}
