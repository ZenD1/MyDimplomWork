using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomZabpo43.classes
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public User(int userId, string email, string phone, string password, int roleId, string roleName)
        {
            UserId = userId;
            Email = email;
            Phone = phone;
            Password = password;
            RoleId = roleId;
            RoleName = roleName;
        }
    }
}
