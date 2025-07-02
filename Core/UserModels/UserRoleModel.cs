namespace Core.UserModels
{
    public class UserRoleModel
    {
        public string Id { get; set; }
        public long MyId { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string? LastUpdatedBy { get; set; }
        public int Status { get; set; }

        public virtual RoleModel Role { get; set; }
        public virtual UserModel User { get; set; }
    }
}