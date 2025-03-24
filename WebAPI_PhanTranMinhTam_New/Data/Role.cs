using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_PhanTranMinhTam_New.Data
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRoles { get; set; }
        public string Name { get; set; }

        public ICollection<User> users { get; set; } = new List<User>();
        public ICollection<Rolepermission> RolePermissions { get; set; } = new List<Rolepermission>();
    }
}
