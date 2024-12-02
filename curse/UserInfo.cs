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

    public static class LocalAdminAccount
    {
        public static readonly string Username = "ladmin";
        public static readonly string Password = "ladmin";
    }
}
