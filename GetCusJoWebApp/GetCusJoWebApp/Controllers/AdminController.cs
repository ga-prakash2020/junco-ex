using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetCusJoWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GetCusJoWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            ViewBag.User = this.User.Identity.Name;
            return View();
        }

        [HttpGet]
        [Route("role/create")]
        public async Task<IActionResult> CreateRole()
        {
            return View("CreateRole");
        }

        [HttpPost]
        [Route("role/create")]
        public async Task<IActionResult> CreateRole(Role roleViewModel)
        {
            if (string.IsNullOrEmpty(roleViewModel.Name)) return BadRequest("Invalid role name");

            var roleName = roleViewModel.Name.Trim();

            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = roleName,
                NormalizedName = roleName
            };

            await this.roleManager.CreateAsync(role);

            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        [Route("list/usersroles")]
        public IActionResult ListUsersAndRoles()
        {
            var roles = this.roleManager.Roles.AsEnumerable();
            var roleViewModel = new List<Role>();
            foreach (var role in roles)
            {
                roleViewModel.Add(new Role { Id = role.Id, Name = role.Name });
            }

            var users = this.userManager.Users.AsEnumerable();
            var userViewModel = new List<User>();
            foreach (var user in users)
            {
                userViewModel.Add(new User { Id = user.Id, Email = user.UserName });
            }

            var usersAndRoles = new UsersAndRoles
            {
                Users = userViewModel.AsEnumerable(),
                Roles = roleViewModel.AsEnumerable()
            };

            var assignRolesViewModel = new AssignRoles
            {
                UsersRoles = usersAndRoles,
                UserRoles = new UserRoles { UserId = "", RoleIds = "" }
            };

            return View("AssignRole", assignRolesViewModel);
        }

        [HttpPost]
        [Route("list/usersroles")]
        public async Task<IActionResult> AssignRoles(AssignRoles assignRoles)
        {
            if(string.IsNullOrEmpty(assignRoles.UserRoles.UserId)) return BadRequest("Please enter the User Id");

            var userId = assignRoles.UserRoles.UserId.Trim();

            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null) return NotFound("User not found");

            if (string.IsNullOrEmpty(assignRoles.UserRoles.RoleIds)) return BadRequest("Please enter the role name");

            var roleList = assignRoles.UserRoles.RoleIds.Trim().Split(',');

            if (roleList.Length == 0) return BadRequest("Please enter atleast one Role");

            foreach (var role in roleList)
            {
                if (string.IsNullOrEmpty(role)) continue;

                if (!await this.userManager.IsInRoleAsync(user, role))
                {
                    await this.userManager.AddToRoleAsync(user, role);
                }
            }

            return RedirectToAction("ListUsersAndRoles");
        }
    }
}