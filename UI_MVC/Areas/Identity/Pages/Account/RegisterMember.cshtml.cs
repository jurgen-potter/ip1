// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.UI.MVC.Areas.Identity.Managers;
using CitizenPanel.UI.MVC.Models;

namespace CitizenPanel.UI.MVC.Areas.Identity.Pages.Account
{
    public class RegisterMemberModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationUserManager _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterMemberModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IDrawManager _drawManager;
        private readonly IPanelManager _panelManager;

        public RegisterMemberModel(
            ApplicationUserManager userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterMemberModel> logger,
            IEmailSender emailSender,
            IDrawManager drawManager,
            IPanelManager panelManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _drawManager = drawManager;
            _panelManager = panelManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required(ErrorMessage = "Voornaam is verplicht.")]
            [Display(Name = "Voornaam")]
            public string FirstName { get; set; }
            
            [Required(ErrorMessage = "Achternaam is verplicht.")]
            [Display(Name = "Achternaam")]
            public string LastName { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Email is verplicht.")]
            [EmailAddress(ErrorMessage = "Dit e-mailadres is ongeldig.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Wachtwoord is verplicht.")]
            [StringLength(100, ErrorMessage = "Het {0} moet tussen de {2} en {1} karakters bevatten.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Wachtwoord")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Bevestig wachtwoord")]
            [Compare("Password", ErrorMessage = "De wachtwoorden komen niet overeen.")]
            public string ConfirmPassword { get; set; }
            
            [Display(Name = "Geslacht")]
            public Gender Gender { get; set; }
            
            [Display(Name = "Geboortedatum")]
            public DateOnly BirthDate { get; set; }
            
            [Display(Name = "Gemeente")]
            public string Town { get; set; }
            
            public List<CriteriaViewModel> CriteriaList { get; set; } = new List<CriteriaViewModel>();
            
            public List<int> SelectedCriteria { get; set; }
            
            public int PanelId { get; set; }
        
            public int InvitationId { get; set; }
        }
        
        public string Code { get; set; }


        public async Task OnGetAsync(string code)
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            Invitation invitation = JsonConvert.DeserializeObject<Invitation>(TempData["Invitation"] as string ?? throw new InvalidOperationException("No valid invitation was given."));
            Input ??= new InputModel();
            
            List<Criteria> extraCriteria = _panelManager.GetExtraCriteriaByPanelId(invitation.PanelId).ToList();
            foreach (Criteria criteria in extraCriteria)
            {
                var criteriaModel = new CriteriaViewModel()
                {
                    Id = criteria.Id,
                    Name = criteria.Name
                };
                foreach (var subCriteria in criteria.SubCriteria)
                {
                    var subCriteriaModel = new SubCriteriaViewModel()
                    {
                        Id = subCriteria.Id,
                        Name = subCriteria.Name
                    };
                    criteriaModel.SubCriteria.Add(subCriteriaModel);
                }
                Input.CriteriaList.Add(criteriaModel);
            }
            
            Input.Email = invitation.Email;
            Input.Gender = invitation.Gender;
            Input.Town = invitation.Town;
            Input.SelectedCriteria = invitation.SelectedCriteria;
            Input.PanelId = invitation.PanelId;
            Input.InvitationId = invitation.Id;
            Code = code;
        }

        public async Task<IActionResult> OnPostAsync(string code)
        {
            var returnUrl = Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateMember();
                user.UserType = UserType.Member;
                var panel = _panelManager.GetPanelById(Input.PanelId);
                user.MemberProfile = new MemberProfile()
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Gender = Input.Gender,
                    BirthDate = Input.BirthDate,
                    Town = Input.Town,
                    Panels = new List<Panel> { panel }
                };
                
                List<SubCriteria> selectedCriteria = new List<SubCriteria>();
                if (Input.SelectedCriteria != null && Input.SelectedCriteria.Count != 0)
                {
                    selectedCriteria.AddRange(Input.SelectedCriteria.Select(subCriteriaId => 
                        _drawManager.GetSubCriteria(subCriteriaId)).Where(subCriteria => subCriteria != null));
                }
                user.MemberProfile.SelectedCriteria = selectedCriteria;

                await _userManager.AddToRoleAsync(user, "Member");
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateWithTenantAsync(user, Input.Password, givenTenantId: panel.TenantId);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    _drawManager.RemoveInvitation(Input.InvitationId);
                    var userId = await _userManager.GetUserIdAsync(user);
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = token, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Bevestig uw email",
                        $"Bevestig uw account door <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>hier te klikken</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName" && Input.Email == user.UserName)
                    {
                        continue;
                    }
                    
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateMember()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/RegisterMember.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
