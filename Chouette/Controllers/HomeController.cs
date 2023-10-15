using Chouette.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Chouette.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {

        private readonly ChouetteIdentityContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;


        public HomeController(ChouetteIdentityContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> IndexAsync()
        {

         

            return View();
        }


        public async Task<IActionResult> ListProducts()
        {
            return View(await _context.Products.ToListAsync());
        }

        public async Task<IActionResult> Update(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return View(product);   
        }


        [HttpPost]
        public async Task<IActionResult> Update(Product p)
        {
             _context.Products.Update(p);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListProducts");
        }

        public async Task<IActionResult> AddProduct()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product p)
        {
            _context.Products.Add(p);
            await _context.SaveChangesAsync();
            return View();
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null) _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListProducts));
        }

        public async Task<IActionResult> SignUp()
        {

            return View(new SignUpModel());
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (ModelState.IsValid)
            {

                AppUser user = new()
                {
                    Email = model.Email,
                    Gender = model.Gender,
                    UserName = model.UserName,
                    ImagePath = "",
                    Name = model.Name,
                    SurName = model.SurName

                };
            
              var identityResult=  await _userManager.CreateAsync(user,model.Password);
               if(identityResult.Succeeded)
                {
                    var memberRole = await _roleManager.FindByNameAsync("Member");
                    if (memberRole == null)
                    {
                        await _roleManager.CreateAsync(new()
                        {
                            Name = "Member",
                            CreatedTime = DateTime.Now

                        });
                    }

                    await _userManager.AddToRoleAsync(user, "Member");
                    RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach(var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View();
        }


        public IActionResult Login(string returnUrl)
        {
            
            return View(new LoginModel() { ReturnUrl=returnUrl});
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {

          
           
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var signInResult=   await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
           
                if (signInResult.Succeeded)
                {

                    if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                  
                   var userRole=await _userManager.GetRolesAsync(user);
                    if (userRole.Contains("Admin"))
                    {
                        return RedirectToAction(nameof(AdminPanel));
                    }else if (userRole.Contains("Member"))
                    {
                        return RedirectToAction(nameof(Panel));
                    }

                }
                else if (signInResult.IsLockedOut)
                {
                    var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                    ModelState.AddModelError("", $"Hesabınız {(lockoutEnd.Value.UtcDateTime - DateTime.UtcNow).Minutes} askıya alınmıştır!");
                }
                else
                {
                    var message = string.Empty;
                    if (user != null)
                    {
                        var failedCount = await _userManager.GetAccessFailedCountAsync(user);
                        message = $"{(_userManager.Options.Lockout.MaxFailedAccessAttempts - failedCount)} kez daha hatalı girerseniz hesabınız kilitlenecek!";
                    }
                    else
                    {
                        message = "Kullanıcı adı veya şifre hatalı!";
                    }
                    ModelState.AddModelError("", message);

                }

               

            }
           
                
            
            return View(model);
        }

        //[Authorize(Roles ="Admin")]//[Authorize(Roles ="Admin,Member")]
        [Authorize]
        public IActionResult GetUserInfo()
        {

            var userName = User.Identity.Name;
            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            return View();
        }


        [Authorize(Roles ="Admin")]
        public IActionResult AdminPanel()
        {
            return View();
        }
        [Authorize(Roles = "Member")]
        public IActionResult Panel()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
          await  _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
