using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptingAndDecrypting
{
    class Program
    {
        static void Main(string[] args)
        {
            string a = Cryptoclass.Encrypt("This is message", "stoyan");
            string b = Cryptoclass.Decrypt(a, "stoyan");
            Console.WriteLine(a);
            Console.WriteLine(b);
        }
    }
}
