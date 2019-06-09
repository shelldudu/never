using System;
using System.Security.Cryptography;
using System.Text;

namespace Never
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static partial class ObjectExtension
    {
        #region 加密

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="pwd">密码字符字节</param>
        /// <param name="hash">加密算法</param>
        /// <returns></returns>
        public static string ComputeHash(byte[] pwd, HashAlgorithm hash)
        {
            byte[] output = hash.ComputeHash(pwd);
            var result = BitConverter.ToString(output);

            return result;
        }

        #endregion 加密

        #region 加密MD5

        /// <summary>
        /// 转换为MD5
        /// </summary>
        /// <param name="password">源密码内容</param>
        /// <returns></returns>
        public static string ToMD5(this string password)
        {
            return ToMD5(password, Encoding.Default);
        }

        /// <summary>
        /// 加密为MD5
        /// </summary>
        /// <param name="password">源密码内容</param>
        /// <param name="encoding">对源密码要要变成字节序列的编码方法</param>
        /// <returns></returns>
        public static string ToMD5(this string password, Encoding encoding)
        {
            if (IsNullOrEmpty(password))
                return password;

            byte[] temp = encoding.GetBytes(password);
            /*MD5静态对象，在多线程下会不安全，所以这里不用static MD5 = 这种静态对象*/
            var md5 = new MD5CryptoServiceProvider();

            return ComputeHash(temp, md5);
        }

        #endregion 加密MD5

        #region 加密SHA1

        /// <summary>
        /// 转换为SHA1
        /// </summary>
        /// <param name="password">源密码内容</param>
        /// <returns></returns>
        public static string ToSHA1(this string password)
        {
            return ToSHA1(password, Encoding.Default);
        }

        /// <summary>
        /// 加密为SHA1
        /// </summary>
        /// <param name="password">源密码内容</param>
        /// <param name="encoding">对源密码要要变成字节序列的编码方法</param>
        /// <returns></returns>
        public static string ToSHA1(this string password, Encoding encoding)
        {
            if (IsNullOrEmpty(password))
                return password;

            byte[] pwd = encoding.GetBytes(password);
            HashAlgorithm sha1 = new SHA1CryptoServiceProvider();

            return ComputeHash(pwd, sha1);
        }

        #endregion 加密SHA1

        #region 加密SHA256

        /// <summary>
        /// 转换为SHA1
        /// </summary>
        /// <param name="password">源密码内容</param>
        /// <returns></returns>
        public static string ToSHA256(this string password)
        {
            return ToSHA256(password, Encoding.Default);
        }

        /// <summary>
        /// 加密为SHA1
        /// </summary>
        /// <param name="password">源密码内容</param>
        /// <param name="encoding">对源密码要要变成字节序列的编码方法</param>
        /// <returns></returns>
        public static string ToSHA256(this string password, Encoding encoding)
        {
            if (IsNullOrEmpty(password))
                return password;

            byte[] pwd = encoding.GetBytes(password);
            HashAlgorithm sha256 = SHA256Managed.Create();

            return ComputeHash(pwd, sha256);
        }

        #endregion 加密SHA256

        #region 加密SHA384

        /// <summary>
        /// 转换为SHA1
        /// </summary>
        /// <param name="password">源密码内容</param>
        /// <returns></returns>
        public static string ToSHA384(this string password)
        {
            return ToSHA384(password, Encoding.Default);
        }

        /// <summary>
        /// 加密为SHA1
        /// </summary>
        /// <param name="password">源密码内容</param>
        /// <param name="encoding">对源密码要要变成字节序列的编码方法</param>
        /// <returns></returns>
        public static string ToSHA384(this string password, Encoding encoding)
        {
            if (IsNullOrEmpty(password))
                return password;

            byte[] pwd = encoding.GetBytes(password);
            HashAlgorithm sha384 = SHA384Managed.Create();

            return ComputeHash(pwd, sha384);
        }

        #endregion 加密SHA384

        #region 加密SHA512

        /// <summary>
        /// 转换为SHA1
        /// </summary>
        /// <param name="password">源密码内容</param>
        /// <returns></returns>
        public static string ToSHA512(this string password)
        {
            return ToSHA512(password, Encoding.Default);
        }

        /// <summary>
        /// 加密为SHA1
        /// </summary>
        /// <param name="password">源密码内容</param>
        /// <param name="encoding">对源密码要要变成字节序列的编码方法</param>
        /// <returns></returns>
        public static string ToSHA512(this string password, Encoding encoding)
        {
            if (IsNullOrEmpty(password))
                return password;

            byte[] pwd = encoding.GetBytes(password);
            HashAlgorithm sha512 = SHA512Managed.Create();

            return ComputeHash(pwd, sha512);
        }

        #endregion 加密SHA512

        #region DES

        /// <summary>
        /// 将源字符通过des加密
        /// </summary>
        /// <param name="text">源字符</param>
        /// <returns></returns>
        public static string ToDES(this string text)
        {
            /*这两个key要跟后面FromDES一致*/
            return ToDES(text, "abcd1234", "4321dcba");
        }

        /// <summary>
        /// 将源字符通过des加密
        /// </summary>
        /// <param name="text">源字符</param>
        /// <param name="key">加密密钥</param>
        /// <param name="iv">偏移量</param>
        /// <returns></returns>
        public static string ToDES(this string text, string key, string iv)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("要加密的内容不能为空");

            if (key.Length != 8)
                throw new ArgumentOutOfRangeException("key的长度要为8个字符");

            if (iv.Length != 8)
                throw new ArgumentOutOfRangeException("key的长度要为8个字符");

            var toEncrypt = Encoding.UTF8.GetBytes(text);

            using (var des = new DESCryptoServiceProvider() { Key = Encoding.ASCII.GetBytes(key), IV = Encoding.ASCII.GetBytes(iv) })
            {
                var bytes = des.CreateEncryptor().TransformFinalBlock(toEncrypt, 0, toEncrypt.Length);
                return BitConverter.ToString(bytes);
            };
        }

        /// <summary>
        /// 从源字符通过des解密
        /// </summary>
        /// <param name="text">源字符</param>
        /// <returns></returns>
        public static string FromDES(this string text)
        {
            /*这两个key要跟后面ToDES一致*/
            return FromDES(text, "abcd1234", "4321dcba");
        }

        /// <summary>
        ///  从源字符通过des解密
        /// </summary>
        /// <param name="text">源字符</param>
        /// <param name="key">加密密钥</param>
        /// <param name="iv">偏移量</param>
        /// <returns></returns>
        public static string FromDES(this string text, string key, string iv)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("要解密的内容不能为空");

            if (key.Length != 8)
                throw new ArgumentOutOfRangeException("key的长度要为8个字符");

            if (iv.Length != 8)
                throw new ArgumentOutOfRangeException("key的长度要为8个字符");

            var toDecrypt = text.Split('-');
            var data = new byte[toDecrypt.Length];
            for (int i = 0; i < toDecrypt.Length; i++)
                data[i] = byte.Parse(toDecrypt[i].ToString(), System.Globalization.NumberStyles.HexNumber);

            using (var des = new DESCryptoServiceProvider() { Key = Encoding.ASCII.GetBytes(key), IV = Encoding.ASCII.GetBytes(iv) })
            {
                var bytes = des.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
                return Encoding.UTF8.GetString(bytes);
            };
        }

        #endregion DES

        #region 3DES

        /// <summary>
        /// 将源字符通过3desc加密
        /// </summary>
        /// <param name="text">明文</param>
        /// <returns></returns>
        public static string To3DES(this string text)
        {
            /*这个key要跟后面From3DES一致*/
            return To3DES(text, "abcd1234ecef1234hijk1234");
        }

        /// <summary>
        /// 将源字符通过3desc加密
        /// </summary>
        /// <param name="data">明文</param>
        /// <returns></returns>
        public static byte[] To3DES(this byte[] data)
        {
            /*这个key要跟后面From3DES一致*/
            return To3DES(data, "abcd1234ecef1234hijk1234");
        }

        /// <summary>
        /// 将源字符通过3desc加密
        /// </summary>
        /// <param name="text">明文</param>
        /// <param name="key">24为字符串</param>
        /// <returns></returns>
        public static string To3DES(this string text, string key)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (key.Length != 24)
                throw new ArgumentOutOfRangeException("key的长度要为24个字符");

            using (var des = new TripleDESCryptoServiceProvider() { Key = ASCIIEncoding.ASCII.GetBytes(key), Mode = CipherMode.ECB })
            {
                var encrypt = des.CreateEncryptor();
                var data = Encoding.UTF8.GetBytes(text);
                return Convert.ToBase64String(encrypt.TransformFinalBlock(data, 0, data.Length));
            }
        }

        /// <summary>
        /// 将源字符通过3desc加密
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="key">24为字符串</param>
        /// <returns></returns>
        public static byte[] To3DES(this byte[] data, string key)
        {
            if (key.Length != 24)
                throw new ArgumentOutOfRangeException("key的长度要为24个字符");

            using (var des = new TripleDESCryptoServiceProvider() { Key = ASCIIEncoding.ASCII.GetBytes(key), Mode = CipherMode.ECB })
            {
                var encrypt = des.CreateEncryptor();
                return encrypt.TransformFinalBlock(data, 0, data.Length);
            }
        }

        /// <summary>
        /// 从源字符通过3desc解密
        /// </summary>
        /// <param name="text">密文</param>
        /// <returns></returns>
        public static string From3DES(this string text)
        {
            /*这个key要跟后面To3DES一致*/
            return From3DES(text, "abcd1234ecef1234hijk1234");
        }

        /// <summary>
        /// 从源字符通过3desc解密
        /// </summary>
        /// <param name="data">密文</param>
        /// <returns></returns>
        public static byte[] From3DES(this byte[] data)
        {
            /*这个key要跟后面To3DES一致*/
            return From3DES(data, "abcd1234ecef1234hijk1234");
        }

        /// <summary>
        /// 从源字符通过3desc解密
        /// </summary>
        /// <param name="text">密文</param>
        /// <param name="key">24为字符串</param>
        /// <returns></returns>
        public static string From3DES(this string text, string key)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (key.Length != 24)
                throw new ArgumentOutOfRangeException("key的长度要为24个字符");

            using (var des = new TripleDESCryptoServiceProvider() { Key = ASCIIEncoding.ASCII.GetBytes(key), Mode = CipherMode.ECB })
            {
                var decrypt = des.CreateDecryptor();
                var data = Convert.FromBase64String(text);
                return Encoding.UTF8.GetString(decrypt.TransformFinalBlock(data, 0, data.Length));
            }
        }

        /// <summary>
        /// 从源字符通过3desc解密
        /// </summary>
        /// <param name="data">密文</param>
        /// <param name="key">24为字符串</param>
        /// <returns></returns>
        public static byte[] From3DES(this byte[] data, string key)
        {
            if (key.Length != 24)
                throw new ArgumentOutOfRangeException("key的长度要为24个字符");

            using (var des = new TripleDESCryptoServiceProvider() { Key = ASCIIEncoding.ASCII.GetBytes(key), Mode = CipherMode.ECB })
            {
                var decrypt = des.CreateDecryptor();
                return decrypt.TransformFinalBlock(data, 0, data.Length);
            }
        }

        #endregion 3DES

        #region RC2

        /// <summary>
        /// 将源字符通过rc2加密
        /// </summary>
        /// <param name="text">明文</param>
        /// <returns></returns>
        public static string ToRC2(this string text)
        {
            /*这个key要跟后面From3DES一致*/
            return ToRC2(text, "abcd1234ecef1234");
        }

        /// <summary>
        /// 将源字符通过rc2加密
        /// </summary>
        /// <param name="text">明文</param>
        /// <param name="key">介于4和16为字符串</param>
        /// <returns></returns>
        public static string ToRC2(this string text, string key)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (!IsBetween(key.Length, 5, 16))
                throw new ArgumentOutOfRangeException("key的长度要介于4和16为字符串");

            using (var des = new RC2CryptoServiceProvider() { Key = ASCIIEncoding.ASCII.GetBytes(key), Mode = CipherMode.ECB })
            {
                var encrypt = des.CreateEncryptor();
                var data = Encoding.UTF8.GetBytes(text);
                return Convert.ToBase64String(encrypt.TransformFinalBlock(data, 0, data.Length));
            }
        }

        /// <summary>
        /// 从源字符通过rc2解密
        /// </summary>
        /// <param name="text">密文</param>
        /// <returns></returns>
        public static string FromRC2(this string text)
        {
            /*这个key要跟后面To3DES一致*/
            return FromRC2(text, "abcd1234ecef1234");
        }

        /// <summary>
        /// 从源字符通过rc2解密
        /// </summary>
        /// <param name="text">密文</param>
        /// <param name="key">16为字符串</param>
        /// <returns></returns>
        public static string FromRC2(this string text, string key)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (!IsBetween(key.Length, 5, 16))
                throw new ArgumentOutOfRangeException("key的长度要介于4和16为字符串");

            using (var des = new RC2CryptoServiceProvider() { Key = ASCIIEncoding.ASCII.GetBytes(key), Mode = CipherMode.ECB })
            {
                var decrypt = des.CreateDecryptor();
                var data = Convert.FromBase64String(text);
                return Encoding.UTF8.GetString(decrypt.TransformFinalBlock(data, 0, data.Length));
            }
        }

        #endregion RC2

        #region base64

        /// <summary>
        /// 进行base64加密
        /// </summary>
        /// <param name="text">加密对象</param>
        /// <returns></returns>
        public static string ToBase64(this string text)
        {
            return ToBase64(text, Encoding.UTF8);
        }

        /// <summary>
        /// 进行base64加密
        /// </summary>
        /// <param name="text">加密对象</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ToBase64(this string text, Encoding encoding)
        {
            var @byte = encoding.GetBytes(text);
            return Convert.ToBase64String(@byte);
        }

        /// <summary>
        /// 进行base64解密
        /// </summary>
        /// <param name="text">已加密对象</param>
        /// <returns></returns>
        public static string FromBase64(this string text)
        {
            return FromBase64(text, Encoding.UTF8);
        }

        /// <summary>
        /// 进行base64解密
        /// </summary>
        /// <param name="text">已加密对象</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string FromBase64(this string text, Encoding encoding)
        {
            var @byte = Convert.FromBase64String(text);
            return encoding.GetString(@byte);
        }

        #endregion base64
    }
}