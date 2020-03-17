using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using InvoiceGenerator.Data;
using InvoiceGenerator.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace InvoiceGenerator.Controllers
{
    [Authorize]
    public class AdministrationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
      //  private readonly ApplicationDbContext _applicationDBcontext;
        private readonly IMapper _mapper;
        private readonly SendGridClient postMail;

        public AdministrationController(UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager,
                                        RoleManager<ApplicationRole> roleManager,
                                    //    ApplicationDbContext applicationDBcontext,
                                        IMapper mapper,
                                        SendGridClient PostMail)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
           // _applicationDBcontext = applicationDBcontext;
            _mapper = mapper;
            postMail = PostMail;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            // note this only works after a user is logged on. 

            return View("AccessDenied");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {   
            return View("Login", new LoginViewModel() {login="",password="", RememberMe=false  });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.login);               
                var result = await _signInManager.PasswordSignInAsync(model.login,model.password,model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl)) 
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View("Login", model);
        }

        [HttpGet] 
        [AllowAnonymous]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult RegisterUser()
        {
            return View("RegisterUser", new RegisterUserViewModel() { Message= "" } );
        }
        [HttpPost]

        [AllowAnonymous]
        public async Task<IActionResult> RegisterUserAsync(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var AU = _mapper.Map<RegisterUserViewModel, ApplicationUser>(model);
                AU.UserName = model.Email;
                var result = await _userManager.CreateAsync(AU, model.Password);
                if (result.Errors.Count() == 0)
                {                
                    ModelState.AddModelError("", "An email has been sent to " + AU.Email + " Please click on Confirm account to activate the account. ");
                    ModelState.AddModelError("", "Note it is possible the email will go into a Spam or Bulk mail trap please check if it does not appear in your inbox. ");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(AU);
                    var CallbackUrl = Url.Action("ConfirmAccount", "Administration", new { UserId = AU.Id, Code = code }, protocol: Request.Scheme);
                    var From = new EmailAddress("Noreply@mail.com", "InvoiceGenerator");
                    var Replyto = new EmailAddress("NoReply@mail.com", "");
                    var To = new EmailAddress(AU.Email, AU.UserName);
                    var Subject = "Activate your account";
                    var HTMLstring = "<H3>  Please confirm your account by clicking this link : <a href=\"" + CallbackUrl + "\"> Confirm Account </a> </h3>";
                    Debug.WriteLine(HTMLstring);
                    var Mail = MailHelper.CreateSingleEmail(From, To, Subject, "", HTMLstring);
                    await postMail.SendEmailAsync(Mail);
                }
                else
                {
                    foreach (var err in result.Errors)
                    {
                        model.Message += err.Description + "/n";
                        ModelState.AddModelError("", err.Description);
                    }
                }
                return View("RegisterUser", model);
            }
            else 
            {
                foreach (var err in ModelState.Values) 
                {
                    model.Message += err.Errors + "/n";
                }
                return View("RegisterUser", model);
            }      
        }

        [HttpGet]
        public async Task<IActionResult> EditUserAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = _mapper.Map<ApplicationUser, EditUserViewModel>(user);
 
            return View("EditUser", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserAsync(EditUserViewModel model)
        {
            var usr = await _userManager.FindByIdAsync(model.Id);
            usr.AddressCode = model.AddressCode;
            usr.AddressL1 = model.AddressL1;
            usr.AddressL2 = model.AddressL2;
            usr.AddressL3 = model.AddressL3;
            usr.AddressL4 = model.AddressL4;
            usr.AddressL5 = model.AddressL5;
            usr.OrganisationEmail = model.OrganisationEmail;
            usr.OrganisationName = model.OrganisationName;
            usr.Person = model.Person;
            var result = await _userManager.UpdateAsync(usr);
            if (!result.Succeeded) 
            {
                foreach (var err in result.Errors) 
                {
                    ModelState.AddModelError("", err.Description);
                }

                return View("EditUser", model);
            }
            return View("EditUser", model);
        }

         [HttpGet]
         [AllowAnonymous]
        public async Task<ActionResult> ConfirmAccount( string UserId,string Code) 
        {
            var AU = await _userManager.FindByIdAsync(UserId);
            await _userManager.ConfirmEmailAsync(AU, Code);
            return RedirectToAction("Index", "Home");
        }

      
    }
}