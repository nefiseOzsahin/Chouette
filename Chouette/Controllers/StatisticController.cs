using Chouette.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chouette.Controllers
{
    public class StatisticController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ChouetteIdentityContext _context;

        public StatisticController(UserManager<AppUser> userManager, ChouetteIdentityContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> SingleStats()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
          
            if (user != null)
            {
             var numberOfSeasons = _context.Entry(user)
            .Collection(u => u.Seasons)
            .Query()
            .Count();

                var numberOfGames = _context.Entry(user)
                    .Collection(u => u.Games)
                    .Query()
                    .Count();

                SingleStatsModel model = new SingleStatsModel
                {
                    NumberOfSeasons = numberOfSeasons,
                    NumberOfGames = numberOfGames
                };
                return View(model);
            }

            return View();
        }

        public IActionResult BinaryStats()
        {
            return View();
        }
    }
}
