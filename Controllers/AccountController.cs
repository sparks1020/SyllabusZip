using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SyllabusZip.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using Microsoft.Extensions.Options;
using SyllabusZip.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SyllabusZip.Common.Data;

namespace SyllabusZip.Controllers
{
    public class AccountController : Controller
    {
        private const string COMPED_CUSTOMER_ID = "cus_1";

        private readonly IMapper _mapper;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        public bool Checked { get; set; }

        public AccountController(IMapper mapper, UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, IEmailSender emailSender, ApplicationDbContext applicationDbContext)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _context = applicationDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginModel userModel, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

            var result = await _signInManager.PasswordSignInAsync(userModel.Email, userModel.Password, userModel.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
            else
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View();
            }

        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(HomeController.Index), "Home");

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync(UserRegistrationModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

            var user = _mapper.Map<UserModel>(userModel);

            var result = await _userManager.CreateAsync(user, userModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return View(userModel);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string emailtitle = "Syllabus Zip: Confirm Your Account";
            string emailbody = "Please confirm your account by clicking on the link below:";
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
            string emailcontents = emailtitle + "<br/>" + emailbody + "<br/>" + confirmationLink;

            await _emailSender.SendEmailAsync(user.Email, "Confirmation email link", emailcontents);

            await _userManager.AddToRoleAsync(user, "Visitor");

            return RedirectToAction(nameof(SuccessRegistration));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View("Error");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }

        [HttpGet]
        public IActionResult Error()
        {
            // TODO: This doesn't actually have a view set up for it, so it renders the Shared Error view, which requires a model.
            // Add new view for "error creating account" in Views/Accounts
            return View();
        }

        [HttpGet]
        public IActionResult SuccessRegistration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // For more information on how to enable account confirmation and password reset please 
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(
                "ResetPassword",
                "Account",
                values: new { token, email = user.Email },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                forgotPasswordModel.Email,
                "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordModel);

            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
                RedirectToAction(nameof(ResetPasswordConfirmation));

            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return View();
            }

            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet("/Join")]
        public IActionResult Join() => View();

        [Authorize]
        public async Task<IActionResult> MyProfile([FromServices] ApplicationDbContext db, [FromServices] IOptions<StripeOptions> stripeOptions)
        {
            var client = new StripeClient(stripeOptions.Value.SecretKey);
            var user = db.Users.FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            DateTime expiration = DateTime.MinValue;
            string sessionUrl = "";

            if (!string.IsNullOrEmpty(user.CustomerId))
            {
                if (user.CustomerId == COMPED_CUSTOMER_ID)
                {
                    expiration = user.Expiration != DateTime.MinValue ? user.Expiration : DateTime.MaxValue;
                }
                else
                {
                    // Get the customer's expiration date
                    var subOptions = new Stripe.SubscriptionListOptions
                    {
                        Customer = user.CustomerId,
                    };
                    var subService = new Stripe.SubscriptionService(client);
                    var subscriptions = await subService.ListAsync(subOptions);

                    // Get the customer's portal URL
                    var options = new Stripe.BillingPortal.SessionCreateOptions
                    {
                        Customer = user.CustomerId,
                        ReturnUrl = $"{Request.Scheme}://{Request.Host}/Account/MyProfile",
                    };
                    var service = new Stripe.BillingPortal.SessionService(client);
                    var session = await service.CreateAsync(options);

                    expiration = subscriptions.Select(sub => sub.CurrentPeriodEnd).Max();
                    sessionUrl = session.Url;
                }
            }

            ViewData["Expiration"] = expiration;
            ViewData["PortalUrl"] = sessionUrl;

            return View();
        }

        public IActionResult CourseStatus()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            SyllabusViewModel syllabusviewmodel = new SyllabusViewModel();
            syllabusviewmodel.Syllabus = _context.Syllabi.Include(s => s.ContactInfo).Where(s => s.UserId == userId).ToList();
            return View(syllabusviewmodel);
        }

        [HttpPost("Account/CourseStatus")]
        public IActionResult CourseStatusResponse()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var syllabi = _context.Syllabi.Include(s => s.ContactInfo).Where(s => s.UserId == userId).ToList();
            foreach(var item in syllabi)
            {
                var name = "s" + item.Id;
                var value = Request.Form[name].FirstOrDefault();
                item.CourseStatus = value != null && (value == "on" || bool.Parse(value));
            }
            _context.SaveChanges();

            SyllabusViewModel syllabusviewmodel = new SyllabusViewModel();
            syllabusviewmodel.Syllabus = syllabi;
            return View("CourseStatus", syllabusviewmodel);
        }
    }
}