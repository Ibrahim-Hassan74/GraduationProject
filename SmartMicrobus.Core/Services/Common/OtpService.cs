using Microsoft.AspNetCore.Identity;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.ServiceContracts.Common;
using System.Security.Cryptography;

namespace SmartMicrobus.Core.Services.Common
{
    public class OtpService : IOtpService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private const string OtpLoginProvider = "OTP";
        private const string OtpTokenName = "AuthOTP";
        private const int OtpBaseCooldownSeconds = 60;


        public OtpService(
            UserManager<ApplicationUser> userManager,
            IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }

        public async Task<string> GenerateOtpAsync(ApplicationUser user)
        {
            var existing = await _userManager
                .GetAuthenticationTokenAsync(user, OtpLoginProvider, OtpTokenName);

            var parsed = TryParseStoredOtp(existing,out _,out var expiryExisting,out var sentExisting,out var resendCountExisting);

            if (parsed && DateTimeOffset.UtcNow <= expiryExisting)
            {
                var cooldownSeconds =
                    OtpBaseCooldownSeconds * Math.Pow(2, resendCountExisting);

                var secondsSinceSent =
                    (DateTimeOffset.UtcNow - sentExisting).TotalSeconds;

                if (secondsSinceSent < cooldownSeconds)
                {
                    var waitSeconds =
                        (int)Math.Ceiling(cooldownSeconds - secondsSinceSent);
                 
                    throw new Exception($"OTP_COOLDOWN:{waitSeconds}");
                }
            }

            // generate otp
            int num = RandomNumberGenerator.GetInt32(0, 1_000_000);
            var otp = num.ToString("D6");

            var hash = _passwordHasher.HashPassword(user, otp);

            var expiry = DateTimeOffset.UtcNow.AddMinutes(5);
            var sentTime = DateTimeOffset.UtcNow;

            var newCount = parsed ? resendCountExisting + 1 : 0;

            var storedValue =
                $"{hash}|{expiry.ToUnixTimeSeconds()}|{sentTime.ToUnixTimeSeconds()}|{newCount}";

            await _userManager.SetAuthenticationTokenAsync(
                user,
                OtpLoginProvider,
                OtpTokenName,
                storedValue);

            return otp;
        }


        public async Task<bool> VerifyOtpAsync(ApplicationUser user, string enteredOtp)
        {
            var stored = await _userManager
                .GetAuthenticationTokenAsync(user, OtpLoginProvider, OtpTokenName);

            if (!TryParseStoredOtp(stored,out var storedHash,out var expiry,out _, out _))
                return false;

            if (DateTimeOffset.UtcNow > expiry)
                return false;

            var result = _passwordHasher.VerifyHashedPassword(user, storedHash,enteredOtp);

            if (result == PasswordVerificationResult.Success)
            {
                await ClearOtpAsync(user);
                return true;
            }

            return false;
        }


        public async Task ClearOtpAsync(ApplicationUser user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, OtpLoginProvider, OtpTokenName);
        }



        private bool TryParseStoredOtp(string? stored, out string hash, out DateTimeOffset expiry, out DateTimeOffset sentTime, out int resendCount)
        {
            hash = string.Empty;
            expiry = default;
            sentTime = default;
            resendCount = 0;

            if (string.IsNullOrEmpty(stored))
                return false;

            var parts = stored.Split('|');
            if (parts.Length != 4)
                return false;

            hash = parts[0];
            expiry = DateTimeOffset.FromUnixTimeSeconds(long.Parse(parts[1]));
            sentTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(parts[2]));
            resendCount = int.Parse(parts[3]);

            return true;
        }


    }
}
