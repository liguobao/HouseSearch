using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace HouseCrawler.Web
{
    /// <summary>
    /// 字符加密类，EncryptString(string Value)加密字符串，DecryptString(string Value)解密字符串。
    /// </summary>
    public static class EncryptionTools
    {

         #region "定义加密字串变量"
        private static SymmetricAlgorithm mCSP = new DESCryptoServiceProvider();  //声明对称算法变量
        private const string CIV = "Mi9l/+7Zujhy12se6Yjy111A";  //初始化向量
        private const string CKEY = "jkHuIy9D/9i="; //密钥（常量）
        #endregion


        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="Value">需加密的字符串</param>
        /// <returns></returns>
        public static  string EncryptString(string Value)
        {
            ICryptoTransform ct; //定义基本的加密转换运算
            MemoryStream ms; //定义内存流
            CryptoStream cs; //定义将内存流链接到加密转换的流
            byte[] byt;
            //CreateEncryptor创建(对称数据)加密对象
            ct = mCSP.CreateEncryptor(Convert.FromBase64String(CKEY),
             Convert.FromBase64String(CIV)); //用指定的密钥和初始化向量创建对称数据加密标准
            byt = Encoding.UTF8.GetBytes(Value); //将Value字符转换为UTF-8编码的字节序列
            ms = new MemoryStream(); //创建内存流
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write); //将内存流链接到加密转换的流
            cs.Write(byt, 0, byt.Length); //写入内存流
            cs.FlushFinalBlock(); //将缓冲区中的数据写入内存流，并清除缓冲区
            cs.Close(); //释放内存流


            return BytesToString(ms.ToArray()); //将内存流转写入字节数组并转换为string字符
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="Value">要解密的字符串</param>
        /// <returns>string</returns>
        public static string DecryptString(string Value)
        {
            ICryptoTransform ct; //定义基本的加密转换运算
            MemoryStream ms; //定义内存流
            CryptoStream cs; //定义将数据流链接到加密转换的流
            byte[] byt;
            ct = mCSP.CreateDecryptor(Convert.FromBase64String(CKEY),
             Convert.FromBase64String(CIV)); //用指定的密钥和初始化向量创建对称数据解密标准

            var bytes = StringToBytes(Value);
            Value = Convert.ToBase64String(bytes);

            byt = Convert.FromBase64String(Value); //将Value(Base 64)字符转换成字节数组
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Encoding.UTF8.GetString(ms.ToArray()); //将字节数组中的所有字符解码为一个字符串
        }


      // DES对称加密

        /// <summary>
        /// Des解密方法
        /// </summary>
        /// <param name="val">待解密字符串，</param>
        /// <param name="key">解密关键key，</param>
        /// <param name="IV">解密向量。</param>
        /// <returns></returns>
        public static string Decrypt(string val, string key, string IV)
        {
            try
            {
                byte[] buffer1 = Convert.FromBase64String(key);
                byte[] buffer2 = Convert.FromBase64String(IV);
                DESCryptoServiceProvider provider1 = new DESCryptoServiceProvider();
                provider1.Mode = CipherMode.ECB;
                provider1.Key = buffer1;
                provider1.IV = buffer2;
                ICryptoTransform transform1 = provider1.CreateDecryptor(provider1.Key, provider1.IV);
                byte[] buffer3 = Convert.FromBase64String(val);
                MemoryStream stream1 = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream1, transform1, CryptoStreamMode.Write);
                stream2.Write(buffer3, 0, buffer3.Length);
                stream2.FlushFinalBlock();
                stream2.Close();
                return Encoding.Default.GetString(stream1.ToArray());
            }
            catch// (System.Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// Des加密方法
        /// </summary>
        /// <param name="val">待解密字符串，</param>
        /// <param name="key">解密关键key</param>
        /// <param name="IV">解密向量</param>
        /// <returns></returns>
        public static string Encrypt(string val, string key, string IV)
        {
            try
            {
                byte[] buffer1 = Convert.FromBase64String(key);
                byte[] buffer2 = Convert.FromBase64String(IV);

                DESCryptoServiceProvider provider1 = new DESCryptoServiceProvider();
                provider1.Mode = CipherMode.ECB;
                provider1.Key = buffer1;
                provider1.IV = buffer2;
                ICryptoTransform transform1 = provider1.CreateEncryptor(provider1.Key, provider1.IV);
                byte[] buffer3 = Encoding.Default.GetBytes(val);
                MemoryStream stream1 = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream1, transform1, CryptoStreamMode.Write);
                stream2.Write(buffer3, 0, buffer3.Length);
                stream2.FlushFinalBlock();
                stream2.Close();
                return Convert.ToBase64String(stream1.ToArray());
            }
            catch// (Exception ex)
            {
                return "";
            }
        }



        // <summary>
        /// byte数组转string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string BytesToString(byte[] bytes)
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
        private static byte[] StringToBytes(string str)
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