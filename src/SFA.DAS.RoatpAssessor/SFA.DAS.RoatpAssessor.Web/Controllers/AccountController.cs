using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAddUserDetailsValidator _addUserDetailsValidator;
        private readonly IRoatpAssessorApiClient _assessorApiClient;

        public AccountController(
            ILogger<AccountController> logger,
            IAddUserDetailsValidator addUserDetailsValidator,
            IRoatpAssessorApiClient assessorApiClient)
        {
            _logger = logger;
            _addUserDetailsValidator = addUserDetailsValidator;
            _assessorApiClient = assessorApiClient;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            _logger.LogInformation("Start of Sign In");
            var redirectUrl = Url.Action("PostSignIn", "Account");
            return Challenge(
                new AuthenticationProperties {RedirectUri = redirectUrl},
                WsFederationDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult PostSignIn()
        {
            //if (!HttpContext.User.HasValidRole())
            //{
            //    _logger.LogInformation($"PostSignIn - User '{HttpContext.User.Identity.Name}' does not have a valid role");
            //    foreach (var cookie in Request.Cookies.Keys)
            //    {
            //        Response.Cookies.Delete(cookie);
            //    }

            //    return RedirectToAction("AccessDenied");
            //}

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult SignOut()
        {
            var callbackUrl = Url.Action("SignedOut", "Account", values: null, protocol: Request.Scheme);

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return SignOut(
                new AuthenticationProperties {RedirectUri = callbackUrl},
                CookieAuthenticationDefaults.AuthenticationScheme,
                WsFederationDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult SignedOut()
        {
            return View("SignedOut");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            if (HttpContext.User != null)
            {
                var userName = HttpContext.User.Identity.Name ?? HttpContext.User.FindFirstValue(ClaimTypes.Upn);
                var roles = HttpContext.User.Claims
                    .Where(c => c.Type == ClaimTypes.Role || c.Type == Domain.Roles.RoleClaimType).Select(c => c.Value);

                _logger.LogError(
                    $"AccessDenied - User '{userName}' does not have a valid role. They have the following roles: '{string.Join(",", roles)}'");
            }

            return View("AccessDenied");
        }

        [HttpGet]
        public IActionResult AddUserDetails()
        {
            return View("~/Views/Account/AddUserDetails.cshtml", new AddUserDetailsModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddUserDetails(AddUserDetailsCommand command)
        {
            // validate
            var validationResponse = await _addUserDetailsValidator.Validate(command);

            if (validationResponse.Errors.Any())
            {
                foreach (var error in validationResponse.Errors)
                {
                    ModelState.AddModelError(error.Field, error.ErrorMessage);
                }
            }

            return View("~/Views/Account/AddUserDetails.cshtml", new AddUserDetailsModel
            {
                ErrorDictionary = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                    ),
                FirstName = command.FirstName,
                LastName = command.LastName
            });
        }
    }
}
