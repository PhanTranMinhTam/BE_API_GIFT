namespace WebAPI_PhanTranMinhTam_New.Models
{
    public class PermisstionRoleDTO
    {
        public int RoleId { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}
