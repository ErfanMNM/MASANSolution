using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR_MASAN_01.Auth
{
    public class UserData
    {
        public string Username { get; set; } = string.Empty;  // Tên user
        public string Password { get; set; }  // Hash password
        public string Salt { get; set; }      // Salt
        public string Role { get; set; }      // Quyền
        public string Key2FA { get; set; }    // Khóa 2FA

        public override string ToString()
        {
            return $"User[Username={Username}, Role={Role}]";
        }
    }
}
