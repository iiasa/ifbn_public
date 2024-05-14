// <copyright file="AccountController.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Controllers
{
	using System.Globalization;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using IFBN.Data.Entities;
	using IFBN.Web.Models;
	using Microsoft.AspNetCore.Authentication;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;

	[Authorize]
	public class AccountController : Controller
	{
		private readonly SignInManager<ApplicationUser> signInManager;

		private readonly UserManager<ApplicationUser> userManager;

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		[AllowAnonymous]
		public IActionResult External(string provider, string returnUrl = null)
		{
			// Request a redirect to the external login provider.
			string redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
			AuthenticationProperties properties = this.signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
			return Challenge(properties, provider);
		}

		// GET: /Account/ExternalLoginCallback
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
		{
			if (remoteError != null)
			{
				ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
				return View(nameof(Login));
			}

			ExternalLoginInfo info = await this.signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				return RedirectToAction(nameof(Login));
			}

			// Sign in the user with this external login provider if the user already has a login.
			Microsoft.AspNetCore.Identity.SignInResult result =
				await this.signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
			if (result.Succeeded)
			{
				////_logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
				return RedirectToLocal(returnUrl);
			}

			// If the user does not have an account, then ask the user to create an account.
			ViewData["ReturnUrl"] = returnUrl;
			ViewData["LoginProvider"] = info.LoginProvider;
			string email = info.Principal.FindFirstValue(ClaimTypes.Email);
			string username = info.Principal.FindFirstValue(ClaimTypes.Name);
			if (username != null)
			{
				username.Replace(" ", string.Empty);
			}
			else
			{ username = string.Empty; }
			return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email, Username = username });
		}

		// POST: /Account/ExternalLoginConfirmation
		[HttpPost]
		[AllowAnonymous]
		////[ValidateAntiForgeryToken]
		public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
		{
			if (ModelState.IsValid)
			{
				// Get the information about the user from the external login provider
				ExternalLoginInfo info = await this.signInManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					return NotFound();
				}

				string email = info.Principal.FindFirstValue(ClaimTypes.Email);
				string username = model.Username;
				string institution = model.Institution;
				string field = model.Field;
				string affiliation = model.Affiliation;
				bool hasAccepted = model.HasAcceptedTerms;

				ApplicationUser user = new ApplicationUser
				{
					UserName = username,
					Email = email,
					Institution = institution,
					Field = field,
					Affiliation = affiliation,
					HasAcceptedTerms = hasAccepted,
				};

				IdentityResult result = await this.userManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await this.userManager.AddLoginAsync(user, info);
					if (result.Succeeded)
					{
						await this.signInManager.SignInAsync(user, false);

						////_logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
						return RedirectToLocal(returnUrl);
					}
				}
			}

			ViewData["ReturnUrl"] = returnUrl;
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<JsonResult> IsUID_Available(string username)
		{
			ApplicationUser user = await this.userManager.FindByNameAsync(username);
			if (user == null)
			{
				return Json(true);
			}

			string suggestedUid = string.Format(CultureInfo.InvariantCulture, "{0} is not available.", username);

			for (int i = 1; i < 100; i++)
			{
				string altCandidate = username + i;
				user = await this.userManager.FindByNameAsync(altCandidate);
				if (user == null)
				{
					suggestedUid = string.Format(CultureInfo.InvariantCulture, "{0} is not available. Try {1}.", username, altCandidate);
					break;
				}
			}

			return Json(suggestedUid);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login()
		{
			return View();
		}

		public async Task<IActionResult> LogOut()
		{
			await HttpContext.SignOutAsync();

			await this.signInManager.SignOutAsync();

			return RedirectToAction("Index", "Home");
		}

		private Task<ApplicationUser> GetCurrentUserAsync()
		{
			return this.userManager.GetUserAsync(HttpContext.User);
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}

			return RedirectToAction("Index", "Home");
		}
	}
}