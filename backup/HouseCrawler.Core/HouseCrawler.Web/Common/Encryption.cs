using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;

namespace HouseCrawler.Web
{
    /// <summary>
    /// 字符加密类，EncryptString(string Value)加密字符串，DecryptString(string Value)解密字符串。
    /// </summary>
    public class EncryptionTools
    {
        private APPConfiguration configuration;

        public EncryptionTools(IOptions<APPConfiguration> configuration)
        {
            this.configuration = configuration.Value;
        }

        private  byte[] GetKey() => StringToBytes(configuration.EncryptionConfigCKEY);
        private  byte[] GetCIV() => StringToBytes(configuration.EncryptionConfigCIV);

        public string Crypt(string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(GetKey(), GetCIV());
            byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }


        // <summary>
        /// byte数组转string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] bytes)
        {
            if (bytes == null)
                return string.Empty;
            return string.Join(string.Empty,
            bytes.Select(b => string.Format("{0:x2}", b)).ToArray());
        }

        /// <summary>
        /// string转byte数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StringToBytes(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            byte[] bytes = new byte[str.Length / 2];
            for (int i = 0; i < str.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte("0x" + str[i] + str[i + 1], 16);
            }
            return bytes;
        }
    }
}