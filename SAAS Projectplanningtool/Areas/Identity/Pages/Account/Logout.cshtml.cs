// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;

namespace software.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly Logger _logger;
        // Custom changes: REMOVED ILogger<LogoutModel> _logger; ADDED: Context && CustomLogger
        public LogoutModel(SignInManager<IdentityUser> signInManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _logger = new Logger(context, signInManager.UserManager);
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            //_logger.LogInformation("User logged out.");
            var userId = _signInManager.UserManager.GetUserId(User);
            await _logger.LogByUserId(userId, "SUCCESS: User logged out.", null);
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}
