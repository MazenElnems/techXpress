using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using techXpress.DataAccess.Entities;
using techXpress.Services.Abstraction;
using techXpress.Services.DTOs.Orders;
using techXpress.UI.ActionRequests;
using techXpress.UI.Models;
using techXpress.UI.VMs.Account;

namespace techXpress.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IOrderManger _orderManger;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager
            , RoleManager<IdentityRole<Guid>> roleManager, IOrderManger orderManger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _orderManger = orderManger;
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

        [Authorize(Roles = UserRole.Customer)]
        [HttpGet]
        public async Task<IActionResult> Profile(string userId)
        {
            User? user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            IEnumerable<GetAllOrdersDto> orderHistory = _orderManger.GetAllOrdersByUserId(Guid.Parse(userId));

            UserProfileVM userProfileVM = new UserProfileVM
            {
                UserId = userId,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                OrderHistory = orderHistory.Select(o => new GetAllOrdersDto
                {
                    Address = o.Address,
                    Carrier = o.Carrier,
                    City = o.City,
                    OrderDate = o.OrderDate,
                    OrderId = o.OrderId,
                    OrderStatus = o.OrderStatus,
                    RecipientPhoneNumber = o.RecipientPhoneNumber,
                    ShippingDate = o.ShippingDate,
                    TotalAmount = o.TotalAmount,
                    TrackingNumber = o.TrackingNumber,
                    UserEmail = o.UserEmail,
                    UserId = o.UserId,
                    PaymentId = o.PaymentId,
                    SessionId = o.SessionId,
                    CouponId = o.CouponId
                }).ToList()
            };

            return View(userProfileVM);
        }

        [Authorize(Roles = UserRole.Customer)]
        [HttpGet]
        public async Task<IActionResult> Settings(string userId)
        {
            User? user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return NotFound();
            }
            UserSettingsVM userSettingsVM = new UserSettingsVM
            {
                UserId = userId,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(userSettingsVM);
        }

        [HttpPost]
        [Authorize(Roles = UserRole.Customer)]
        public async Task<IActionResult> Settings(UserSettingsVM userSettingsVM)
        {
            if (ModelState.IsValid)
            {
                User? user = await _userManager.FindByIdAsync(userSettingsVM.UserId);
                if (user == null)
                {
                    return NotFound();
                }
                user.UserName = userSettingsVM.UserName;
                user.Email = userSettingsVM.Email;
                user.PhoneNumber = userSettingsVM.PhoneNumber;
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // User Enter New Password
                    if (userSettingsVM.NewPassword != null)
                    {
                        IdentityResult identityResult = await _userManager.ChangePasswordAsync(user, userSettingsVM.CurrentPassword!, userSettingsVM.NewPassword!);
                        if (identityResult.Succeeded)
                        {
                            TempData["Success"] = "Password changed successfully.";
                        }
                        else
                        {
                            foreach (var error in identityResult.Errors)
                            {
                                ModelState.AddModelError(error.Code, error.Description);
                            }
                            return View(userSettingsVM); // Return to same view if model is invalid
                        }
                    }
                    return RedirectToAction(nameof(Profile), new { userId = userSettingsVM.UserId });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }
            return View(userSettingsVM);
        }

        [HttpPost]
        [Authorize(Roles = UserRole.Customer)]
        public async Task<IActionResult> ChangePassword(UserSettingsVM userSettingsVM)
        {
            if (!ModelState.IsValid)
            {
                return View("Settings", userSettingsVM); // Return to same view if model is invalid
            }

            User? user = await _userManager.FindByIdAsync(userSettingsVM.UserId);
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, userSettingsVM.CurrentPassword!, userSettingsVM.NewPassword!);
            if (result.Succeeded)
            {
                TempData["Success"] = "Password changed successfully.";
                return RedirectToAction(nameof(Profile), new { userId = userSettingsVM.UserId });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Settings", userSettingsVM);
        }

        [HttpGet]
        public IActionResult AccessDenied()
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
