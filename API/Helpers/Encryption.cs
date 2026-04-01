using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Helpers
{
    public class Encryption
    {
        public static string getMd5Hash(string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        public static string Generate(int PassLength)
        {
            string GeneratedPassword = string.Empty;

            string PossibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            try
            {
                char[] possibleChars = PossibleChars.ToCharArray();
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < PassLength; i++)
                {
                    // Get our cryptographically random 32-bit integer & use as seed in Random class
                    // NOTE: random value generated PER ITERATION, meaning that the System.Random class
                    // is re-instantiated every iteration with a new, crytographically random numeric seed.
                    int randInt32 = GetRandomInt();
                    Random r = new Random(randInt32);

                    int nextInt = r.Next(possibleChars.Length);
                    char c = possibleChars[nextInt];
                    builder.Append(c);
                }

                GeneratedPassword = builder.ToString();
                return GeneratedPassword;
            }
            catch (Exception)
            {
                //An error has occurred while trying to generate random password! 
                //Technical description: ex.Message.ToString()
                return string.Empty;
            }
        }

        public static Int32 GetRandomInt()
        {
            byte[] randomBytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            Int32 randomInt = BitConverter.ToInt32(randomBytes, 0);
            return randomInt;
        }
    }
}
