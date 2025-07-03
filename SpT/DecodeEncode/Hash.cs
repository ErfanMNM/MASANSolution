using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SpT
{
    public static class HashHelper
    {
        /// <summary>
        /// Tính hash cho file với thuật toán tuỳ chọn (MD5, SHA1, SHA256, SHA512).
        /// </summary>
        public static string ComputeFileHash(string filePath, string algorithm = "MD5")
        {
            using (var stream = File.OpenRead(filePath))
            using (var hasher = HashAlgorithm.Create(algorithm))
            {
                if (hasher == null)
                    throw new InvalidOperationException($"Hash algorithm {algorithm} not supported!");

                var hash = hasher.ComputeHash(stream);
                return ByteArrayToHexString(hash);
            }
        }

        /// <summary>
        /// Tính hash cho chuỗi string.
        /// </summary>
        public static string ComputeStringHash(string text, string algorithm = "MD5", Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;

            using (var hasher = HashAlgorithm.Create(algorithm))
            {
                if (hasher == null)
                    throw new InvalidOperationException($"Hash algorithm {algorithm} not supported!");

                var data = encoding.GetBytes(text);
                var hash = hasher.ComputeHash(data);
                return ByteArrayToHexString(hash);
            }
        }

        /// <summary>
        /// Tính hash cho byte[].
        /// </summary>
        public static string ComputeBytesHash(byte[] data, string algorithm = "MD5")
        {
            using (var hasher = HashAlgorithm.Create(algorithm))
            {
                if (hasher == null)
                    throw new InvalidOperationException($"Hash algorithm {algorithm} not supported!");

                var hash = hasher.ComputeHash(data);
                return ByteArrayToHexString(hash);
            }
        }

        /// <summary>
        /// Chuyển byte[] thành hex string.
        /// </summary>
        private static string ByteArrayToHexString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2")); // lower-case hex
            return sb.ToString();
        }
    }

  
}
