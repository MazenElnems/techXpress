using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using techXpress.DataAccess.Entities;
using techXpress.UI.ActionRequests;
using techXpress.UI.Models;

namespace techXpress.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager
            , RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login([FromQuery] string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserActionRequest request, string? ReturnUrl)
        {
            // 1. validate the request.
            // 2. check if the user exists in db.
            // 3. check if the password is correct.
            //      if correct => sign in the user (create a cookie for the user).
            //      if not correct => show error message.
            // 4. redirect the user to the ReturnUrl or to the home page.

            if (ModelState.IsValid)
            {
                User? user = await _userManager.FindByEmailAsync(request.Email);

                if(user != null)
                {
                    bool isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

                    if (isPasswordValid)
                    {
                        // it will send set-cookie header in the next response.
                        await _signInManager.SignInAsync(user, request.RememberMe);

                        if(ReturnUrl != null)
                        {
                            return Redirect(ReturnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("InvalidCredentials", "Invalid email or password.");
            return View(request);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(CreateUserActionRequest request)
        {
            if (ModelState.IsValid) 
            {
                User user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = request.UserName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    PasswordHash = request.Password
                };


                // 1. save user in the db.
                IdentityResult result = await _userManager.CreateAsync(user, request.Password);
                await _userManager.AddToRoleAsync(user, UserRole.Customer);     // add customer role by default

                // 2. check if the user is saved successfully.
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }
            }
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // it sends set-cookie header with empty value.
            await _signInManager.SignOutAsync();  
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Profile()
        {
            return View();
        }

    }
    public static class UserRole
    {
        public const string Admin = "Admin";
        public const string Seller = "Seller";
        public const string Customer = "Customer";
    }
}
