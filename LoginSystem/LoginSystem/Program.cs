using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LoginSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] accountInfo = UI.RequestLogin();

            byte[] usernameHash = ComputeHash(accountInfo[0]);
            byte[] passwordHash = ComputeHash(accountInfo[1]);

            if (Db.CheckAccount(usernameHash, passwordHash))
            {
                Console.WriteLine("You succesfully logged!");
            }
            else
            {
                Console.WriteLine("False login information!");
            }

        }

        static byte[] ComputeHash(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);

            var shaM = new SHA512Managed();
            return shaM.ComputeHash(data);
        }
    }
}
