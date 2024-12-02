using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace curse
{
    public static class UserInfo
    {
        public static int id;
        public static string userName;
        public static string email;
        public static string password;
        public static int role;
    }

    public class LocalAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
