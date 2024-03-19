using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services.Impls
{
    public static class CryptoService
    {
        public static string BlowfishKey { get; set; } = "SomeSecretKey";

        public static string ComputeSha256Hash(string? data)
        {
            if (data == null)
            {
                return "";
            }

            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(data));
            var strBuilder = new StringBuilder();

            foreach (var _ in bytes)
            {
                strBuilder.Append(_.ToString("x2"));
            }

            return strBuilder.ToString();
        }

        public static string Blowfish(string? data, bool encrypt = true)
        {
            if (data == null)
            {
                return "";
            }

            var dataBytes = encrypt ? Encoding.UTF8.GetBytes(data) : Convert.FromBase64String(data);
            var cipher = new BufferedBlockCipher(new CbcBlockCipher(new BlowfishEngine()));

            while (dataBytes.Length % 8 != 0)
            {
                dataBytes = [..dataBytes, 0];
            }

            cipher.Init(encrypt, new KeyParameter(Encoding.UTF8.GetBytes(BlowfishKey)));

            var outputDataBytes = new byte[cipher.GetOutputSize(dataBytes.Length)];
            var length = cipher.ProcessBytes(dataBytes, 0, dataBytes.Length, outputDataBytes, 0);

            cipher.DoFinal(outputDataBytes, length);

            return encrypt ? Convert.ToBase64String(outputDataBytes) : Encoding.UTF8.GetString(outputDataBytes);
        }
    }
}
