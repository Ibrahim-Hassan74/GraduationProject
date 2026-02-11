namespace SmartMicrobus.Core.Helper
{
    internal static class AuthHelper
    {
        public static bool TryParseStoredOtp(string? stored, out string otp, out DateTimeOffset expiryTime, out DateTimeOffset? sentTime, out int resendCount)
        {
            otp = string.Empty;
            expiryTime = DateTimeOffset.MinValue;
            sentTime = null;
            resendCount = 0;

            if (string.IsNullOrWhiteSpace(stored))
                return false;

            var parts = stored.Split('|');
            if (parts.Length < 2)
                return false;

            otp = parts[0];

            if (!long.TryParse(parts[1], out var expirySeconds))
                return false;

            expiryTime = DateTimeOffset.FromUnixTimeSeconds(expirySeconds);

            if (parts.Length >= 3 && long.TryParse(parts[2], out var sentSeconds))
            {
                sentTime = DateTimeOffset.FromUnixTimeSeconds(sentSeconds);
            }

            if (parts.Length >= 4 && int.TryParse(parts[3], out var count))
            {
                resendCount = Math.Max(0, count);
            }

            return true;
        }
    }
}
