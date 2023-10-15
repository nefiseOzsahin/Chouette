using Chouette.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chouette.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly ChouetteIdentityContext _context;
        private readonly RoleManager<AppRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, ChouetteIdentityContext context, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
      
        public async Task<IActionResult> Index()
        {
            // var query = _userManager.Users;
            //var users = _context.Users.Join(_context.UserRoles, user => user.Id, userRole => userRole.UserId, (user, userRole) => new
            //{
            //    user,
            //    userRole

            //}).Select(x => new AppUser
            //{
            //    Id = x.user.Id,
            //    AccessFailedCount = x.user.AccessFailedCount,
            //    ConcurrencyStamp = x.user.ConcurrencyStamp,
            //    Email = x.user.Email,
            //    EmailConfirmed = x.user.EmailConfirmed,
            //    Gender = x.user.Gender,
            //    ImagePath = x.user.ImagePath,
            //    LockoutEnabled = x.user.LockoutEnabled,
            //    LockoutEnd = x.user.LockoutEnd,
            //    NormalizedEmail = x.user.NormalizedEmail,
            //    NormalizedUserName = x.user.NormalizedUserName,
            //    PasswordHash = x.user.PasswordHash,
            //    PhoneNumber = x.user.PhoneNumber,
            //    PhoneNumberConfirmed = x.user.PhoneNumberConfirmed,
            //    SecurityStamp = x.user.SecurityStamp,
            //    TwoFactorEnabled = x.user.TwoFactorEnabled,
            //    UserName = x.user.UserName                
            //}).ToList();

            //var users = _context.Users.Join(_context.UserRoles, user => user.Id, userRole => userRole.UserId, (user, userRole) => new
            //{
            //    user,
            //    userRole

            //}).Join(_context.Roles, two => two.userRole.RoleId, role => role.Id, (two, role) => new
            //{
            //    two.user,
            //    two.userRole,
            //    role

            //}).Where(x => x.role.Name != "Admin").Select(x => new AppUser
            //{
            //    Id = x.user.Id,
            //    AccessFailedCount = x.user.AccessFailedCount,
            //    ConcurrencyStamp = x.user.ConcurrencyStamp,
            //    Email = x.user.Email,
            //    EmailConfirmed = x.user.EmailConfirmed,
            //    Gender = x.user.Gender,
            //    ImagePath = x.user.ImagePath,
            //    LockoutEnabled = x.user.LockoutEnabled,
            //    LockoutEnd = x.user.LockoutEnd,
            //    NormalizedEmail = x.user.NormalizedEmail,
            //    NormalizedUserName = x.user.NormalizedUserName,
            //    PasswordHash = x.user.PasswordHash,
            //    PhoneNumber = x.user.PhoneNumber,
            //    PhoneNumberConfirmed = x.user.PhoneNumberConfirmed,
            //    SecurityStamp = x.user.SecurityStamp,
            //    TwoFactorEnabled = x.user.TwoFactorEnabled,
            //    UserName = x.user.UserName
            //}).ToList();
            //var users =await _userManager.GetUsersInRoleAsync("Member");
            //doğrudan ilgili entityi geriye dönmemeliyiz doğru değil ama yapılabilir.
            //return View(users);

            //var filteredUsers=new List<AppUser>();
            //var users = _userManager.Users.ToList();
            //foreach (var user in users)
            //{
            //    var roles = await _userManager.GetRolesAsync(user);
            //    if (!roles.Contains("Admin"))
            //    {
            //        filteredUsers.Add(user);
            //    }
            //}

            //return View(filteredUsers);

            var users = _userManager.Users.ToList();
            return View(users);
        }

        public IActionResult Create()
        {
            return View(new SeasonSignUpModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SeasonSignUpModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Email = model.SignUpModel.Email,
                    UserName = model.SignUpModel.UserName,
                    Gender = model.SignUpModel.Gender,
                    ImagePath="",
                    Name=model.SignUpModel.Name,
                    SurName=model.SignUpModel.SurName


                };
                var result = await _userManager.CreateAsync(user, model.SignUpModel.UserName + "123");
                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(user, "Member");
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);

                    }
                }
                
            }
            return View(new SignUpModel());
        }

       
        public async Task<IActionResult> AssignRole(int userId,string username)
        {

            var user=_userManager.Users.SingleOrDefault(x=>x.Id== userId);
            var userRoles=await _userManager.GetRolesAsync(user);
            var roles = _roleManager.Roles.ToList();
            RoleAssignSendModel model=new RoleAssignSendModel();
            List<RoleAssignListModel> list=new List<RoleAssignListModel>();
            foreach (var role in roles)
            {
                list.Add(new()
                {
                    Name = role.Name,
                    RoleId = role.Id,
                    Exist = userRoles.Contains(role.Name)
                });
            }
            model.Roles=list;
            model.UserId= userId;
            model.UserName= username;
            return View(model);
        }

       
        [HttpPost]
        public async Task<IActionResult> AssignRole(RoleAssignSendModel model)
        {
            //rol ekleme: seçilen rolun ilgili userda olmaması gerek
            //rol çıkarma: seçilen rolun ilgili userda olması gerek

            var user = _userManager.Users.SingleOrDefault(x => x.Id == model.UserId);
            var userRoles=await _userManager.GetRolesAsync(user);

            foreach (var role in model.Roles)
            {
                if (role.Exist)
                {
                    if (!userRoles.Contains(role.Name))
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                }
                else
                {
                    if (userRoles.Contains(role.Name))
                    {
                        await _userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateUserAsync(int userId)
        {
            var user =await _userManager.Users.SingleOrDefaultAsync(x=>x.Id == userId);
         
            return View(user);
        }

       
        [HttpPost]
        public async Task<IActionResult> UpdateUserAsync(AppUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            await _userManager.UpdateSecurityStampAsync(user);


            user.Name = model.Name??"";
            user.Email = model.Email;
            user.Gender = model.Gender;
            user.PhoneNumber = model.PhoneNumber;
            user.SurName = model.SurName??"";
            user.UserName = model.UserName;

            
            var result =await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            // Update successful, redirect or return success message
            return RedirectToAction("Index");
        }

      
        public async Task<IActionResult> DeleteUserAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                 
                    return RedirectToAction("Index"); 
                }
                else
                {
                    return NotFound();
                }
            }

            return View("Index");
        }

    }
}
