using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BealeCipher
{
    class BealeCipher
    {
        private string key;
        private string plainText = "";
        private string cipherText = "";

        public BealeCipher(string key)
        {
            this.key = key;
        }
        public BealeCipher(string key, string plainText)
        {
            this.key = key;
            this.plainText = plainText;
        }

        public void encrypt()
        {
            string[] keyArray = key.Split(' ');
            string[] plainTextArray = plainText.Split(' ');
            for (int i = 0; i < plainTextArray.Length; i++)
            {
                string word = plainTextArray[i];
                int indexOfWord = Array.IndexOf(keyArray, word);
                if (indexOfWord < 0)
                {
                    throw new Exception();
                }

                cipherText += indexOfWord + 1;
                cipherText += " ";
            }

            cipherText.TrimEnd();

        }
        public string decrypt(string cipherText)
        {
            int[] indexOfWord = cipherText
                            .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(n => int.Parse(n) - 1)
                            .ToArray();
            string[] keyArray = key.Split(' ');
            foreach (int index in indexOfWord)
            {
                plainText += keyArray[index] + " ";
            }

            return plainText.TrimEnd();
        }

        public string getCipherText()
        {
            return this.cipherText;
        }
    }
}
