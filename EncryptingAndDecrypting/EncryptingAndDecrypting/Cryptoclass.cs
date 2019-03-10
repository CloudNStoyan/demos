using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptingAndDecrypting
{
    public static class Cryptoclass
    {
        public static string Encrypt(string text, string key)
        {
            string result = "";

            int keyCount = 0;
            foreach (var letter in text)
            {
                char letterOfKey = key[keyCount];

                result += (char)(letter + letterOfKey);


                if ((keyCount + 1) < (key.Length - 1))
                {
                    keyCount++;
                }
                else
                {
                    keyCount = 0;
                }
            }

            return result;
        }

        public static string Decrypt(string text, string key)
        {
            string result = "";

            int keyCount = 0;
            foreach (var letter in text)
            {
                char letterOfKey = key[keyCount];

                result += (char)(letter - letterOfKey);


                if ((keyCount + 1) < (key.Length - 1))
                {
                    keyCount++;
                }
                else
                {
                    keyCount = 0;
                }
            }

            return result;
        }
    }
}
