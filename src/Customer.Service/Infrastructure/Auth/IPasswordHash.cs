namespace Customer.Service.Infrastructure.Auth
{
    public interface IPasswordHash
    {
        /// <summary>
        /// Hash Password with Salt.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public string CreateSaltedPassword(string password, string salt);

        string CreatePasswordSalt(string platformHash, string userPassword);

        /// <summary>
        /// Compare Byte Arrays in String format
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        bool CompareByteArrayStrings(string str1, string str2);

        /// <summary>
        /// Compares to byte[] if they are equal
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        bool CompareByteArrays(byte[] array1, byte[] array2);

        /// <summary>
        /// Convert string to byte array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        byte[] StringToByteArray(string str);


        /// <summary>
        /// Convert string to byte array
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        string ByteArrayToString(byte[] bytes);
    }
}
