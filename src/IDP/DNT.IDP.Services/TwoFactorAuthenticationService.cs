using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DNT.IDP.Services
{
    public interface ITwoFactorAuthenticationService
    {
        Task SendTemporaryCodeAsync(string subjectId);
        Task<bool> IsValidTemporaryCodeAsync(string subjectId, string code);
    }

    public class TwoFactorAuthenticationService : ITwoFactorAuthenticationService
    {
        private const string TwoFactorCodeClaimType = "idsrv.2FA";
        private const string ExpirationDateClaimType = "idsrv.2FA.Expiration";
        private const int TemporaryCodeExpirationHours = 2;

        private readonly IUserClaimsService _userClaimsService;
        private readonly IRandomNumberProvider _randomNumberProvider;
        private readonly ILogger<TwoFactorAuthenticationService> _logger;

        public TwoFactorAuthenticationService(
            IUserClaimsService userClaimsService,
            IRandomNumberProvider randomNumberProvider,
            ILogger<TwoFactorAuthenticationService> logger)
        {
            _userClaimsService = userClaimsService;
            _randomNumberProvider =
                randomNumberProvider ?? throw new ArgumentNullException(nameof(randomNumberProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendTemporaryCodeAsync(string subjectId)
        {
            var randomCode = _randomNumberProvider.Next();
            await saveTwoFactorCodeClaimsAsync(subjectId, randomCode);
            await sendCodeToUserAsync(subjectId, randomCode);
        }

        private async Task sendCodeToUserAsync(string subjectId, int randomCode)
        {
            var userEmail = await _userClaimsService.GetUserClaimAsync(subjectId, "email");
            // TODO: replace it with send_email or send_sms
            _logger.LogInformation($"Hello {userEmail}! Your TwoFactorCode is: {randomCode}");
        }

        public async Task<bool> IsValidTemporaryCodeAsync(string subjectId, string code)
        {
            var twoFactorCodeClaim = await _userClaimsService.GetUserClaimAsync(subjectId, TwoFactorCodeClaimType);
            var expirationDateClaim = await _userClaimsService.GetUserClaimAsync(subjectId, ExpirationDateClaimType);
            return twoFactorCodeClaim != null &&
                   expirationDateClaim != null &&
                   twoFactorCodeClaim.ClaimValue == code &&
                   DateTime.Parse(expirationDateClaim.ClaimValue).ToUniversalTime() >= DateTime.UtcNow;
        }

        private async Task saveTwoFactorCodeClaimsAsync(string subjectId, int randomCode)
        {
            var expirationDate =
                DateTime.UtcNow.AddHours(TemporaryCodeExpirationHours).ToString("o", CultureInfo.InvariantCulture);
            await _userClaimsService.AddOrUpdateUserClaimValuesAsync(subjectId,
                new List<(string ClaimType, string ClaimValue)>
                {
                    (TwoFactorCodeClaimType, randomCode.ToString()),
                    (ExpirationDateClaimType, expirationDate)
                });
        }
    }
}