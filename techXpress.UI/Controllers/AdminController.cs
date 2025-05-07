using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using techXpress.DataAccess.Entities;
using techXpress.UI.Models;
using Microsoft.EntityFrameworkCore;
using techXpress.UI.ActionRequests;
using techXpress.DataAccess.Data;

namespace techXpress.UI.Controllers
{
    [Authorize(Roles = UserRole.Admin)]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<(User User, IList<string> Roles)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add((user, roles));
            }

            return View(userList);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserActionRequest request)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = request.UserName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(request.Role))
                    {
                        await _userManager.AddToRoleAsync(user, request.Role);
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var model = new UpdateUserActionRequest
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = userRoles.FirstOrDefault()
            };

            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UpdateUserActionRequest request)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(request.Id.ToString());
                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = request.UserName;
                user.Email = request.Email;
                user.PhoneNumber = request.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Update roles
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!string.IsNullOrEmpty(request.Role))
                    {
                        await _userManager.AddToRoleAsync(user, request.Role);
                    }

                    // Reset password only if both fields are provided and match
                    if (!string.IsNullOrEmpty(request.NewPassword) && !string.IsNullOrEmpty(request.ConfirmNewPassword))
                    {
                        if (request.NewPassword == request.ConfirmNewPassword)
                        {
                            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                            var passwordResult = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
                            if (!passwordResult.Succeeded)
                            {
                                foreach (var error in passwordResult.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                                ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
                                return View(request);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "The new password and confirmation password do not match.");
                            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
                            return View(request);
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                // Debug: Log ModelState errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var model = new UpdateUserActionRequest
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = userRoles.FirstOrDefault()
            };

            return View("DeleteUser", model);
        }

        [HttpPost]
        [ActionName("DeleteUser")]
        public async Task<IActionResult> ConfirmDeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<(User User, IList<string> Roles)>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                userList.Add((u, roles));
            }
            return View("Index", userList);
        }

        [HttpPost]
        public async Task<IActionResult> BulkDeleteUsers(List<Guid> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var id in userIds)
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}