using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Models;

namespace Restaurant.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminUserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserWithRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserWithRoleViewModel
                {
                    User = user,
                    Role = roles.FirstOrDefault() ?? "User"
                });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.NewRole))
                return Json(new { success = false, message = "Invalid request data." });

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return Json(new { success = false, message = "User not found." });

            // 
            if (user.Email == "admin@restaurant.com")
                return Json(new { success = false, message = "You cannot change the role of the main admin." });

            if (!await _roleManager.RoleExistsAsync(model.NewRole))
                return Json(new { success = false, message = "Role does not exist." });

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, model.NewRole);

            return Json(new { success = true, message = $"{user.Email} role changed to {model.NewRole}." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Json(new { success = false, message = "User not found." });

            //
            if (user.Email == "admin@restaurant.com")
                return Json(new { success = false, message = "You cannot delete the main admin account." });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return Json(new { success = false, message = "Error deleting user." });

            return Json(new { success = true, message = $"{user.Email} deleted successfully." });
        }


    }

    public class ChangeRoleModel
    {
        public string UserId { get; set; }
        public string NewRole { get; set; }
    }

    public class UserWithRoleViewModel
    {
        public AppUser User { get; set; }
        public string Role { get; set; }
    }
}
