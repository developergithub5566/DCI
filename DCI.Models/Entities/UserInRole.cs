using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class UserInRole : IAuditable
    {
        public int UserInRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
