using System.Security.Cryptography;
using System.Text;

namespace Customer.Service.Infrastructure.Auth
{
    /// <summary>
    /// Class used for password encryption
    /// See: https://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
    /// 2nd example is here: https://stackoverflow.com/questions/3063116/how-to-easily-salt-a-password-in-a-c-sharp-windows-form-application
    /// </summary>
    public class PasswordHash: IPasswordHash
    {
        /// <summary>
        /// Hash Password with Salt.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public string CreateSaltedPassword(string password, string salt)
        {
            var hashedPassword = GenerateSaltedHash(password, salt);
            return ByteArrayToString(hashedPassword);
        }

        public string CreatePasswordSalt(string platformHash, string userEmail) => $"{userEmail}_{platformHash}_{Guid.NewGuid()}";


        /// <summary>
        /// Compare Byte Arrays in String format
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public bool CompareByteArrayStrings(string str1, string str2)
        {
            byte[] array1 = StringToByteArray(str1);
            byte[] array2 = StringToByteArray(str2);
            return CompareByteArrays(array1, array2);
        }

        /// <summary>
        /// Compares to byte[] if they are equal
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if(array1.Length != array2.Length)
            {
                return false;
            }

            for(int i = 0; i < array1.Length; i++)
            {
                if(array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Convert string to byte array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] StringToByteArray(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// Convert string to byte array
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string ByteArrayToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Generate Salted Hash
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        private byte[] GenerateSaltedHash(string plainText, string salt)
        {
            var passwordBytes = StringToByteArray(plainText);
            var saltHash = StringToByteArray(salt);

            var algorithm = SHA256.Create();

            byte[] plainTextWithSaltBytes =
              new byte[plainText.Length + salt.Length];

            for(int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = passwordBytes[i];
            }
            for(int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = saltHash[i];
            }
            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

    }
}
