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
            string[] wordArray = key.Split(' ');
            string[] plainTextArray = plainText.Split(' ');
            for (int i = 0; i < plainTextArray.Length; i++)
            {
                string word = plainTextArray[i];
                int index = Array.IndexOf(wordArray, word);
                if (index < 0)
                {
                    throw new Exception();
                }

                cipherText += index + 1;
                cipherText += " ";
            }

            cipherText.TrimEnd();

        }
        public string decrypt(string cipherText)
        {
            int[] indexes = cipherText
                            .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(n => int.Parse(n) - 1)
                            .ToArray();
            string[] wordArray = key.Split(' ');
            foreach (int index in indexes)
            {
                plainText += wordArray[index] + " ";
            }

            return plainText.TrimEnd();
        }

        public string getCipherText()
        {
            return this.cipherText;
        }
    }
}
