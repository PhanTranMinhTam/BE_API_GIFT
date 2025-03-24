﻿namespace WebAPI_PhanTranMinhTam_New.Models
{
    public class CreateDTO
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public int Point { get; set; }
        public int RoleId { get; set; }
        public IFormFile? Image { get; set; }
    }
}
