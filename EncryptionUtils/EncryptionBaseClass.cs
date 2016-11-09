    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;
    
    /// <summary>
    /// Encryption Base Class
    /// </summary>
    public class BaseClass
    {
        #region Numeric Function Utils
        /// <summary>
        /// function find the first number evenly divisible by the multiple starting with the seed value.
        /// </summary>
        /// <param name="multiple"></param>
        /// <param name="seed"></param>
        // ReSharper disable InconsistentNaming
        internal static int GetLCM(int multiple, int seed)
        // ReSharper restore InconsistentNaming
        {
            while (seed % multiple > 0) { seed++; }
            return seed;
        }

        /// <summary>
        /// This function generates a Random Number between an upper and lower bound input seeds
        /// </summary>
        /// <param name="lBound">Lower Bound</param>
        /// <param name="uBound">Upper Bound</param>
        /// <returns>A Randomly generated number (Int32)</returns>
        internal int GetCryptographicRandomNumber(int lBound, int uBound)
        {
            try
            {
                if (lBound < 0 || lBound > uBound || lBound == uBound - 1)
                {
                    throw new ArgumentException("Lower bound is out of range.", "lBound");
                }
                if (uBound < 0 || uBound < lBound)
                {
                    throw new ArgumentException("Upper bound is out of range.", "uBound");
                }
               
                var rng = new RNGCryptoServiceProvider();
                uint urndnum;
                var rndnum = new Byte[4];

                uint xcludeRndBase = (uint.MaxValue - (uint.MaxValue % (uint)(uBound - lBound)));
                do
                {
                    rng.GetBytes(rndnum);
                    urndnum = BitConverter.ToUInt32(rndnum, 0);
                } while (urndnum >= xcludeRndBase);

                return (int)(urndnum % (uBound - lBound)) + lBound;
            }
            catch (CryptographicException ex)
            {
                throw new ApplicationException("error during generation of RNG random number.", ex);
            }
            catch (Exception e)
            {
                throw new ApplicationException("random number generation.", e);
            }
        }

        /// <summary>
        /// Is String Numeric Utility Function
        /// </summary>
        /// <param name="stringOfDigits"></param>
        /// <returns></returns>
        protected static bool IsStringNumeric(string stringOfDigits)
        {
            try
            {
                bool isInt = false;
                char[] numbers = stringOfDigits.ToCharArray();
                foreach (char n in numbers)
                {
                    int retVal;
                    isInt = int.TryParse(n.ToString(CultureInfo.InvariantCulture), out retVal);
                    if (isInt == false)
                    {
                        break;
                    }
                }
                return isInt;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("error in numeric string check.", ex);
            }
        }
        #endregion

        #region DES Encryption/Decryption
        /// <summary>
        /// Generic DES Encryption function
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="padding"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        internal static byte[] Encrypt (byte[] data, byte[] key, byte[] iv, PaddingMode padding, CipherMode mode)
        {
            try
            {
                var algorithm = new DESCryptoServiceProvider { Padding = padding, Mode = mode };               
                ICryptoTransform encryptor = algorithm.CreateEncryptor(key, iv);
                return Crypter(data, key, iv, encryptor);
            }
            catch (CryptographicException ex)
            {
                throw new ApplicationException("Error during des encryption of license code.", ex);
            }
        }

        /// <summary>
        /// Triple DES Encryption function used by SkyVision
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="padding"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        internal static byte[] TripleDESEncrypt(byte[] data, byte[] key, byte[] iv, PaddingMode padding, CipherMode mode)
        {
            try
            {
                var algorithm = new TripleDESCryptoServiceProvider { Padding = padding, Mode = mode };
                ICryptoTransform encryptor = algorithm.CreateEncryptor(key, iv);
                var retVal = Crypter(data, key, iv, encryptor);
                encryptor.Dispose();
                algorithm.Dispose();
                return retVal;
            }
            catch (CryptographicException ex)
            {
                throw new ApplicationException("Error during Triple DES encryption of license code.", ex);
            }
        }
        /// <summary>
        ///  Generic DES Decryption Function
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="padding"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        internal static byte[] TripleDESDecrypt(byte[] data, byte[] key, byte[] iv, PaddingMode padding, CipherMode mode)
        {
            try
            {
                var algorithm = new TripleDESCryptoServiceProvider { Padding = padding, Mode = mode };
                ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv);
                var retVal = Crypter(data, key, iv, decryptor);
                decryptor.Dispose();
                algorithm.Dispose();
                return retVal;
            }
            catch (CryptographicException ex)
            {
                throw new ApplicationException("Error during Triple DES decryption of license code.", ex);
            }
        }


        /// <summary>
        ///  Generic DES Decryption Function
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="padding"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        internal static byte[] Decrypt(byte[] data, byte[] key, byte[] iv, PaddingMode padding, CipherMode mode)
        {
            try
             {
                var algorithm = new DESCryptoServiceProvider {Padding = padding, Mode = mode};
                ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv);
                var retVal = Crypter(data, key, iv, decryptor);
                decryptor.Dispose(); 
                algorithm.Dispose();
                return retVal;
             }
             catch (CryptographicException ex)
             {
                 throw new ApplicationException("Error during des decryption of license code.", ex);
             }
        }

        /// <summary>
        /// Encryption in Memory
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="cryptor"></param>
        /// <returns></returns>
        internal static byte[] Crypter (byte[] data, byte[] key, byte[] iv, ICryptoTransform cryptor)
        {
            try
            {
                var m = new MemoryStream();
                var c = new CryptoStream(m, cryptor, CryptoStreamMode.Write);
                c.Write(data, 0, data.Length);
                c.FlushFinalBlock();
                return m.ToArray();
            }
            catch (CryptographicException ex)
            {
                throw new ApplicationException("Error during in memory encryption with des algorithm.", ex);
            }
        }
        /// <summary>
        /// Overloaded method to return encrypted base 64 string
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="padding"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        internal static string Encrypt(string data, byte[] key, byte[] iv, PaddingMode padding, CipherMode mode)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(data), key, iv, padding, mode));
        }
        /// <summary>
        /// Overloaded method to return plain decrypted text 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="padding"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        internal static string Decrypt(string data, byte[] key, byte[] iv, PaddingMode padding, CipherMode mode)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(data), key, iv, padding, mode));
        }
        #endregion

        #region MD5 HASH
        /// <summary>
        /// Standard MD5 Hash function
        /// </summary>
        /// <param name="input">String Input</param>
        /// <returns></returns>
        public static string GetMD5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.Default.GetBytes(input));

            var s = new StringBuilder();
            foreach (byte b in data)
            {
                s.AppendFormat("{0:x2}", b);
            }
           
            return s.ToString();
        }

        /// <summary>
        /// Standard MD5 Hash
        /// </summary>
        /// <param name="input">Binary Input</param>
        /// <returns></returns>
        public static string GetMD5Hash(byte[] input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(input);

            var s = new StringBuilder();
            foreach (byte b in data)
            {
                s.AppendFormat("{0:x2}", b);
            }

            return s.ToString();
        }
        #endregion
    }