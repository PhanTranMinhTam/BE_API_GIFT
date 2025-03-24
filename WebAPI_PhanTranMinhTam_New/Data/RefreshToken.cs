using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_PhanTranMinhTam_New.Data
{
    [Table("RefreshToken")]
    public class RefreshToken
    {
        [Key]
        public Guid IdRefreshToken { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User user { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? IssuedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }
}
