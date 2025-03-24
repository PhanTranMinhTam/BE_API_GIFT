using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_PhanTranMinhTam_New.Data
{
    public class UserActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? UserId { get; set; }
        //[ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public int Points { get; set; }
        public int Rank { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
