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
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            alphabet += alphabet.ToUpperInvariant();
            string reversealphabet = new string(alphabet.Reverse().ToArray());

            var encryptedBuilder = new StringBuilder();

            int keyCount = 0;
            foreach (var letter in text)
            {
                char letterOfKey = key[keyCount];

                int indexOf = text.IndexOf(letter);


                int index = letter + indexOf;
                index += letterOfKey;
                
                index -= reversealphabet.IndexOf(letterOfKey);
                encryptedBuilder.Append((char)index);

                if ((keyCount + 1) < (key.Length - 1))
                {
                    keyCount++;
                }
                else
                {
                    keyCount = 0;
                }
            }

            return encryptedBuilder.ToString();
        }

        public static string Decrypt(string text, string key)
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            alphabet += alphabet.ToUpperInvariant();
            string reversealphabet = new string(alphabet.Reverse().ToArray());

            var decryptedBuilder = new StringBuilder();

            int keyCount = 0;
            foreach (var letter in text)
            {
                char letterOfKey = key[keyCount];

                int indexOf = text.IndexOf(letter);


                int index = letter + indexOf;
                index -= letterOfKey;

                index -= reversealphabet.IndexOf(letterOfKey);
                Console.WriteLine(index);
                decryptedBuilder.Append((char)index);

                if ((keyCount + 1) < (key.Length - 1))
                {
                    keyCount++;
                }
                else
                {
                    keyCount = 0;
                }
            }

            return decryptedBuilder.ToString();
        }
    }
}
