using System;
using System.Threading.Tasks;
using DNT.IDP.Common;
using DNT.IDP.Controllers.Account;
using DNT.IDP.DomainClasses;
using DNT.IDP.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace DNT.IDP.Controllers.UserRegistration
{
    public class UserRegistrationController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IIdentityServerInteractionService _interaction;

        public UserRegistrationController(IUsersService usersService,
            IIdentityServerInteractionService interaction)
        {
            _usersService = usersService;
            _interaction = interaction;
        }

        [HttpGet]
        public IActionResult RegisterUser(string returnUrl)
        {
            var vm = new RegisterUserViewModel {ReturnUrl = returnUrl};
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // ModelState invalid, return the view with the passed-in model
                // so changes can be made
                return View(model);
            }

            // create user + claims
            var userToCreate = new User
            {
                Password = model.Password.GetSha256Hash(),
                Username = model.Username,
                IsActive = true
            };
            userToCreate.UserClaims.Add(new UserClaim("country", model.Country));
            userToCreate.UserClaims.Add(new UserClaim("address", model.Address));
            userToCreate.UserClaims.Add(new UserClaim("given_name", model.Firstname));
            userToCreate.UserClaims.Add(new UserClaim("family_name", model.Lastname));
            userToCreate.UserClaims.Add(new UserClaim("email", model.Email));
            userToCreate.UserClaims.Add(new UserClaim("subscriptionlevel", "FreeUser"));

            // add it through the repository
            await _usersService.AddUserAsync(userToCreate);

            // log the user in
            // issue authentication cookie with subject ID and username
            var props = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
            };
            await HttpContext.SignInAsync(userToCreate.SubjectId, userToCreate.Username, props);


            // continue with the flow     
            if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return Redirect("~/");
        }
    }
}