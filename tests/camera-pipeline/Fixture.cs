using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CameraPipeline.QA
{
    internal sealed class Fixture
    {
        public const int Population = 200000;
        public string[] Codes;
        public string CodesSha256;

        public static Fixture Create(string directory)
        {
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, "codes.txt");
            using (var writer = new StreamWriter(path, false, new UTF8Encoding(false)))
                for (int i = 0; i < Population; i++)
                    writer.WriteLine(Code(i));
            string[] codes = File.ReadAllLines(path, Encoding.UTF8);
            if (codes.Length != Population || new HashSet<string>(codes, StringComparer.Ordinal).Count != Population)
                throw new InvalidOperationException("Fake PO must contain exactly 200000 unique codes after reload.");
            if (!codes.Select((c, i) => c == Code(i)).All(x => x))
                throw new InvalidOperationException("Fake PO round-trip mismatch.");
            string digest;
            using (var sha = SHA256.Create())
            using (var stream = File.OpenRead(path))
                digest = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
            Json.Write(Path.Combine(directory, "po.json"), new {
                orderNo = "QA_SANDBOX_200000", gtin = "QA_SYNTHETIC_ONLY", orderQty = Population,
                count = codes.Length, distinctCount = codes.Distinct(StringComparer.Ordinal).Count(),
                codesFile = "codes.txt", sha256 = digest,
                boundary = "Synthetic file PO; production constructors, DB loaders, sockets and hardware are never loaded."
            });
            return new Fixture { Codes = codes, CodesSha256 = digest };
        }

        private static string Code(int i)
        {
            // One member exercises all production text-token normalizations; no newline characters.
            return i == 0 ? "QA\u001DGS\u001ERS\u001FUS000000000000" : "QA" + i.ToString("D22", CultureInfo.InvariantCulture);
        }
    }
}
