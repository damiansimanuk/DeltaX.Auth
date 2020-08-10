using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DeltaX.Auth.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using IdentityServer4.Services;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using IdentityModel;
using IdentityServer4.Extensions;

namespace DeltaX.Auth.IdentityServer.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        IIdentityServerInteractionService _interaction;



        public LogoutModel(
            SignInManager<ApplicationUser> signInManager,
            ILogger<LogoutModel> logger,
            IIdentityServerInteractionService interaction)
        {
            _signInManager = signInManager;
            _logger = logger;
            _interaction = interaction;
        }

        [BindProperty]
        public string LogoutId { get; set; }

        public async Task OnGet(string logoutId)
        {
            LogoutId = logoutId;
        }

        public async Task<IActionResult> OnPostOld(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }


        public async Task<IActionResult> OnPost(string logoutId, string returnUrl = null)
        {
            LogoutRequest logout = null;
            if (!string.IsNullOrWhiteSpace(logoutId))
            {
                logout = await _interaction.GetLogoutContextAsync(logoutId);
            }

            if (User?.Identity.IsAuthenticated == true)
            {
                await _signInManager.SignOutAsync();

                // raise the logout event
                // await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // // check if we need to trigger sign-out at an upstream identity provider
            // if (logout!=null && logout.TriggerExternalSignout)
            // {
            //     // build a return URL so the upstream provider will redirect back
            //     // to us after the user has logged out. this allows us to then
            //     // complete our single sign-out processing.
            //     string url = Url.Action("Logout", new { logoutId = InputLogout.LogoutId });
            // 
            //     // this triggers a redirect to the external provider for sign-out
            //     return SignOut(new AuthenticationProperties { RedirectUri = url }, InputLogout.ExternalAuthenticationScheme);
            // }

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            if (!string.IsNullOrEmpty(logoutId))
            {
                return RedirectToPage("LoggedOut", new { logoutId = logoutId });
            }

            return RedirectToPage("/");
        }
    }
}
