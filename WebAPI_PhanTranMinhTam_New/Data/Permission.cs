using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_PhanTranMinhTam_New.Data
{
    public class Permission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPermission { get; set; }
        public string Name { get; set; }
        public ICollection<Rolepermission> RolePermissions { get; set; } = new List<Rolepermission>();
    }
}
