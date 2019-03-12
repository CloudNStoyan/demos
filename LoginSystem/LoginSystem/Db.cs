using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LoginSystem
{
    public static class Db
    {
        public static bool CheckAccount(byte[] usernameHash, byte[] passwordHash)
        {
            byte[] dbUsername = Encoding.ASCII.GetBytes("admin");
            byte[] dbPassword = Encoding.ASCII.GetBytes("test");

            var shaM = new SHA512Managed();

            byte[] dbUsernameHash = shaM.ComputeHash(dbUsername);
            byte[] dbPasswordHash = shaM.ComputeHash(dbPassword);

            return dbUsernameHash == usernameHash && dbPasswordHash == passwordHash;
        }
    }
}
