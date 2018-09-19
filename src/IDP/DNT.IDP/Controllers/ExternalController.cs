using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using DNT.IDP.DomainClasses;
using DNT.IDP.Services;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DNT.IDP.Controllers.Account
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class ExternalController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IEventService _events;
        private readonly IUsersService _usersService;
        private readonly ILogger<ExternalController> _logger;
        private readonly ITwoFactorAuthenticationService _twoFactorAuthenticationService;

        public ExternalController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IEventService events,
            IUsersService usersService,
            ILogger<ExternalController> logger,
            ITwoFactorAuthenticationService twoFactorAuthenticationService)
        {
            _interaction = interaction;
            _clientStore = clientStore;
            _events = events;
            _usersService = usersService;
            _logger = logger;
            _twoFactorAuthenticationService = twoFactorAuthenticationService;
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Challenge(string provider, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }

            if (AccountOptions.WindowsAuthenticationSchemeName == provider)
            {
                _logger.LogInformation("windows authentication needs special handling");
                return await ProcessWindowsLoginAsync(returnUrl);
            }
            else
            {
                _logger.LogInformation("start challenge and roundtrip the return URL and scheme");
                var props = new AuthenticationProperties
                {
                    RedirectUri = Url.Action(nameof(Callback)),
                    Items =
                    {
                        {"returnUrl", returnUrl},
                        {"scheme", provider},
                    }
                };

                return Challenge(props, provider);
            }
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            // read external identity from the temporary cookie
            var result =
                await HttpContext.AuthenticateAsync(IdentityServer4.IdentityServerConstants
                    .ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            // retrieve return URL
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            // lookup our user and external provider info
            var (user, provider, providerUserId, claims) = await FindUserFromExternalProvider(result);

            foreach (var claim in claims)
            {
                _logger.LogInformation($"External provider[{provider}] info-> claim:{claim.Type}, value:{claim.Value}");
            }

            if (user == null)
            {
                // user wasn't found by provider, but maybe one exists with the same email address?  
                if (provider == "Google" || provider == "Facebook")
                {
                    // email claim from Google
                    var email = claims.FirstOrDefault(c =>
                        c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                    if (email != null)
                    {
                        var userByEmail = await _usersService.GetUserByEmailAsync(email.Value);
                        if (userByEmail != null)
                        {
                            // add Google as a provider for this user
                            await _usersService.AddUserLoginAsync(userByEmail.SubjectId, provider, providerUserId);

                            // redirect to ExternalLoginCallback
                            var continueWithUrlAfterAddingUserLogin =
                                Url.Action("Callback", new {returnUrl = returnUrl});
                            return Redirect(continueWithUrlAfterAddingUserLogin);
                        }
                    }
                }


                var returnUrlAfterRegistration = Url.Action("Callback", new {returnUrl = returnUrl});
                var continueWithUrl = Url.Action("RegisterUser", "UserRegistration",
                    new {returnUrl = returnUrlAfterRegistration, provider = provider, providerUserId = providerUserId});
                return Redirect(continueWithUrl);
            }

            // 2-F.A
            var id = new ClaimsIdentity();
            id.AddClaim(new Claim(JwtClaimTypes.Subject, user.SubjectId));
            await HttpContext.SignInAsync(scheme: Startup.TwoFactorAuthenticationScheme,
                principal: new ClaimsPrincipal(id));

            await _twoFactorAuthenticationService.SendTemporaryCodeAsync(user.SubjectId);

            var redirectToAdditionalFactorUrl =
                Url.Action("AdditionalAuthenticationFactor",
                    new
                    {
                        returnUrl = returnUrl,
                        rememberLogin = false,
                        provider = provider,
                        providerUserId = providerUserId
                    });

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(redirectToAdditionalFactorUrl);
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                return Redirect("~/");
            }

            // user might have clicked on a malicious link - should be logged
            throw new Exception("invalid return URL");
        }

        [HttpGet]
        public IActionResult AdditionalAuthenticationFactor(
            string returnUrl, bool rememberLogin, string provider, string providerUserId)
        {
            // create VM
            var vm = new AdditionalAuthenticationFactorViewModel
            {
                RememberLogin = rememberLogin,
                ReturnUrl = returnUrl,
                Provider = provider,
                ProviderUserId = providerUserId
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdditionalAuthenticationFactor(
            AdditionalAuthenticationFactorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // read identity from the temporary cookie
            var info = await HttpContext.AuthenticateAsync(Startup.TwoFactorAuthenticationScheme);
            var tempUser = info?.Principal;
            if (tempUser == null)
            {
                throw new Exception("2FA error");
            }

            // ... check code for user
            if (!await _twoFactorAuthenticationService.IsValidTemporaryCodeAsync(tempUser.GetSubjectId(), model.Code))
            {
                ModelState.AddModelError("code", "2FA code is invalid.");
                return View(model);
            }

            // this allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.

            // read external identity from the temporary cookie
            var result =
                await HttpContext.AuthenticateAsync(IdentityServer4.IdentityServerConstants
                    .ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);
            ProcessLoginCallbackForWsFed(result, additionalLocalClaims, localSignInProps);
            ProcessLoginCallbackForSaml2p(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            var user = await _usersService.GetUserBySubjectIdAsync(tempUser.GetSubjectId());
            await _events.RaiseAsync(new UserLoginSuccessEvent(model.Provider, model.ProviderUserId, user.SubjectId,
                user.Username));
            await HttpContext.SignInAsync(user.SubjectId, user.Username, model.Provider, localSignInProps,
                additionalLocalClaims.ToArray());

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // delete temporary cookie used for 2FA
            await HttpContext.SignOutAsync(Startup.TwoFactorAuthenticationScheme);

            // check if external login is in the context of an OIDC request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (context != null)
            {
                if (await _clientStore.IsPkceClientAsync(context.ClientId))
                {
                    // if the client is PKCE then we assume it's native, so this change in how to
                    // return the response is for better UX for the end user.
                    return View("Redirect", new RedirectViewModel {RedirectUrl = model.ReturnUrl});
                }
            }

            return Redirect(model.ReturnUrl);
        }


        private async Task<IActionResult> ProcessWindowsLoginAsync(string returnUrl)
        {
            // see if windows auth has already been requested and succeeded
            var result = await HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
            if (result?.Principal is WindowsPrincipal wp)
            {
                // we will issue the external cookie and then redirect the
                // user back to the external callback, in essence, treating windows
                // auth the same as any other external authentication mechanism
                var props = new AuthenticationProperties()
                {
                    RedirectUri = Url.Action("Callback"),
                    Items =
                    {
                        {"returnUrl", returnUrl},
                        {"scheme", AccountOptions.WindowsAuthenticationSchemeName},
                    }
                };

                var id = new ClaimsIdentity(AccountOptions.WindowsAuthenticationSchemeName);
                id.AddClaim(new Claim(JwtClaimTypes.Subject, wp.Identity.Name));
                id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));

                // add the groups as claims -- be careful if the number of groups is too large
                if (AccountOptions.IncludeWindowsGroups)
                {
                    var wi = wp.Identity as WindowsIdentity;
                    var groups = wi.Groups.Translate(typeof(NTAccount));
                    var roles = groups.Select(x => new Claim(JwtClaimTypes.Role, x.Value));
                    id.AddClaims(roles);
                }

                await HttpContext.SignInAsync(
                    IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    new ClaimsPrincipal(id),
                    props);
                return Redirect(props.RedirectUri);
            }
            else
            {
                // trigger windows auth
                // since windows auth don't support the redirect uri,
                // this URL is re-triggered when we call challenge
                return Challenge(AccountOptions.WindowsAuthenticationSchemeName);
            }
        }

        private async Task<(User user, string provider, string providerUserId, IList<Claim> claims)>
            FindUserFromExternalProvider(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            // find external user
            var user = await _usersService.GetUserByProviderAsync(provider, providerUserId);

            return (user, provider, providerUserId, claims);
        }

        private void ProcessLoginCallbackForOidc(AuthenticateResult externalResult, List<Claim> localClaims,
            AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var id_token = externalResult.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                localSignInProps.StoreTokens(new[] {new AuthenticationToken {Name = "id_token", Value = id_token}});
            }
        }

        private void ProcessLoginCallbackForWsFed(AuthenticateResult externalResult, List<Claim> localClaims,
            AuthenticationProperties localSignInProps)
        {
        }

        private void ProcessLoginCallbackForSaml2p(AuthenticateResult externalResult, List<Claim> localClaims,
            AuthenticationProperties localSignInProps)
        {
        }
    }
}