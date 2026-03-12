using Microsoft.Extensions.Configuration;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.ServiceContracts.Common;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SmartMicrobus.Core.Services.Common
{
    public class QrTokenService : IQrTokenService
    {
        private readonly byte[] _aesKey;

        public QrTokenService(IConfiguration config)
        {
            _aesKey = Convert.FromBase64String(config["QrToken:AesKey"]);
        }

        public string GenerateToken(MicrobusQrPayload payload)
        {
            payload.IssuedAt = DateTime.UtcNow;

            var json = JsonSerializer.Serialize(payload);
            var plaintext = Encoding.UTF8.GetBytes(json);

            var nonce = RandomNumberGenerator.GetBytes(12);
            var ciphertext = new byte[plaintext.Length];
            var tag = new byte[16];

            using var aes = new AesGcm(_aesKey);

            aes.Encrypt(nonce, plaintext, ciphertext, tag);

            var packed = new byte[12 + 16 + ciphertext.Length];

            nonce.CopyTo(packed, 0);
            tag.CopyTo(packed, 12);
            ciphertext.CopyTo(packed, 28);

            return Convert.ToBase64String(packed)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        public MicrobusQrPayload? DecryptToken(string token)
        {
            try
            {
                var base64 = token.Replace('-', '+').Replace('_', '/')
                    + new string('=', (4 - token.Length % 4) % 4);

                var packed = Convert.FromBase64String(base64);

                var nonce = packed[..12];
                var tag = packed[12..28];
                var ciphertext = packed[28..];

                var plaintext = new byte[ciphertext.Length];

                using var aes = new AesGcm(_aesKey);

                aes.Decrypt(nonce, ciphertext, tag, plaintext);

                var payload = JsonSerializer.Deserialize<MicrobusQrPayload>(
                    Encoding.UTF8.GetString(plaintext));

                if (payload!.ExpiresAt < DateTime.UtcNow)
                    return null;

                return payload;
            }
            catch
            {
                return null;
            }
        }
    }
}