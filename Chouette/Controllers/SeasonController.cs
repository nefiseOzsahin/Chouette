using Chouette.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Chouette.Controllers
{
    [Authorize(Roles = "Member,Admin")]
    public class SeasonController : Controller
    {

        private readonly ChouetteIdentityContext _context;
        private readonly UserManager<AppUser> _userManager;


        public SeasonController(ChouetteIdentityContext context, UserManager<AppUser> userManager)
        {

            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            List<Season> seasons = _context.Seasons.ToList();


            return View(seasons);
        }



        public IActionResult CreateUser()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateUserAsync(SeasonSignUpModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Email = model.SignUpModel.Email,
                    UserName = model.SignUpModel.UserName,
                    Gender = model.SignUpModel.Gender,
                    ImagePath = "",
                    Name = model.SignUpModel.Name,
                    SurName = model.SignUpModel.SurName


                };
                var result = await _userManager.CreateAsync(user, model.SignUpModel.UserName + "123");
                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(user, "Member");
                    TempData["SuccessMessage"] = "User registered successfully!";
                    return RedirectToAction("CreateSeason");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);

                    }
                }
            }

            return View();
        }


        public async Task<IActionResult> CreateSeasonAsync()
        {
            List<AppUser> users = _context.Users.ToList();

            Season season = new()
            {
                Users = users,
                //Date = DateTime.Now

            };

            SeasonSignUpModel model = new()
            {
                Season = season
            };

            return View(model);
            ;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSeasonAsync(SeasonSignUpModel model, int[] Users)
        {
            model.Season.Name = model.Season.Name + seasonNumberCalculateAsync();
            if (ModelState.IsValid)
            {
                // Save or update Season
                if (model.Season.Id == 0)
                {
                    model.Season.CreateDate = DateTime.Now;

                    foreach (int item in Users)
                    {

                        model.Season.Users.Add(await _userManager.FindByIdAsync(item.ToString()));
                    }



                    _context.Seasons.Add(model.Season);
                }
                else
                {
                    _context.Seasons.Update(model.Season);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // Redirect to index or another appropriate action
            }
            else
            {

                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
            }

            List<AppUser> users = _context.Users.ToList();

            model.Season.Users = users;

            return View(model);
            ;
        }

        private int seasonNumberCalculateAsync()
        {
            var seasonCount = _context.Seasons.Count();


            return seasonCount + 1;
        }

        public async Task<IActionResult> SeasonUserListAsync(int seasonId)
        {

            var targetSeason = _context.Seasons
                .Include(s => s.Users)
                .FirstOrDefault(s => s.Id == seasonId);


            SeasonAddUser model = new()
            {
                Season = targetSeason,
                Users = _context.Users.ToList().Except(targetSeason.Users.ToList()).ToList()
            };


            return View(model);
        }


        public async Task<IActionResult> EnterPoints(int seasonId)
        {
            var season = _context.Seasons.Include(s => s.Users).FirstOrDefault(s => s.Id == seasonId);

            var userPoints = HttpContext.Session.GetObject<Dictionary<int, int>>("UserPoints");

            var usersForSelectedSeason = season.Users.Select(u => new
            UserViewModel
            {
                UserId = u.Id,
                UserName = u.UserName,
                Point = userPoints?.ContainsKey(u.Id) == true ? userPoints[u.Id] : 0,
                TotalPoint = _context.Scores
            .Where(score => score.AppUserId == u.Id && score.Game.SeasonId == seasonId)
            .Sum(score => score.Point)
            }).ToList();

            var viewModel = new ScoreInputViewModel
            {
                Season = season,
                Users = usersForSelectedSeason


            };

            //özet



            var usersForSelectedSeasonPlayed = season.Users.Select(u => new
            PointViewModel
            {
                UserId = u.Id,
                UserName = u.UserName,
                Points = _context.Scores
             .Where(score => score.AppUserId == u.Id && score.Game.SeasonId == seasonId)
             .Select(score => new PointAndGame
             {
                 Point = score.Point,
                 GameNo = score.Game.Id
             })
             .ToList(),
                GameNo = _context.Scores
        .Count(score => score.AppUserId == u.Id && score.Game.SeasonId == seasonId)
            }).ToList();



         

            var viewModel2 = new ScoreDetailViewModel
            {
                Season = season,
                Users = usersForSelectedSeasonPlayed


            };

            //özet end

            int maxGameNo = 0; // Set a default value

            try
            {
                maxGameNo = _context.Scores
                    .Where(score => score.Game.SeasonId == seasonId)
                    .GroupBy(score => score.AppUserId)
                    .Select(group => new
                    {
                        UserId = group.Key,
                        GameNo = group.Count()
                    })
                    .Max(user => user.GameNo);

                // Other code to prepare your model

                //total
                var viewModel3 = new ScoreInputDetailViewModel
                {
                    scoreInputViewModel = viewModel,
                    scoreDetailViewModel = viewModel2,
                    maxGameNo = maxGameNo

                };
                //total end

                return View(viewModel3);
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or use the default value

                // Other code to prepare your model with default values

                //total
                var viewModel3 = new ScoreInputDetailViewModel
                {
                    scoreInputViewModel = viewModel,
                    scoreDetailViewModel = viewModel2,
                    maxGameNo = maxGameNo

                };
                //total end

                return View(viewModel3);
            }



            
        }




        [HttpPost]
        public async Task<IActionResult> SubmitPoints(ScoreInputDetailViewModel model)
        {


            int totalPoints = model.scoreInputViewModel.Users.Sum(user => user.Point);

            Dictionary<int, int> userPoints = new Dictionary<int, int>();

            if (totalPoints == 0)
            {
                var newGame = new Game
                {
                    SeasonId = model.scoreInputViewModel.Season.Id,
                    Name = "New Game"
                };
                _context.Games.Add(newGame);
                _context.SaveChanges();

                foreach (var item in model.scoreInputViewModel.Users)
                {
                    var score = new Score
                    {
                        GameId = newGame.Id,
                        AppUserId = item.UserId,
                        Point = item.Point
                    };

                    _context.Scores.Add(score);

                }
                _context.SaveChanges();

                foreach (var userViewModel in model.scoreInputViewModel.Users)
                {
                    userViewModel.Point = 0;
                }

                HttpContext.Session.SetObject("UserPoints", new Dictionary<int, int>());

                return RedirectToAction("EnterPoints", new { seasonId = model.scoreInputViewModel.Season.Id });
            }
            else
            {
                foreach (var userViewModel in model.scoreInputViewModel.Users)
                {
                    userPoints[userViewModel.UserId] = userViewModel.Point;
                }
                HttpContext.Session.SetObject("UserPoints", userPoints);
                TempData["ErrorMessage"] = "Toplam 0 olamlı. Kontrol edin lütfen!";
                return RedirectToAction("EnterPoints", new { seasonId = model.scoreInputViewModel.Season.Id });
            }

        }

        [HttpPost]
        public async Task<IActionResult> SeasonAddUser(SeasonAddUser model, int[] Users)
        {

            if (!ModelState.IsValid)
            {
                Season season = await _context.Seasons.FirstOrDefaultAsync(x => x.Id == model.Season.Id);
                foreach (int item in Users)
                {
                    season.Users.Add(await _userManager.FindByIdAsync(item.ToString()));
                }

                _context.Seasons.Update(season);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction("SeasonUserList", new { seasonId = model.Season.Id });
        }



        public async Task<IActionResult> SeasonDeleteUser(int userId, int seasonId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            Season season = await _context.Seasons.Include(s => s.Users).FirstOrDefaultAsync(x => x.Id == seasonId);
            if (season != null && user != null)
            {
                season.Users.Remove(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Kullanıcı başarıyla kaldırıldı!";
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanıcı veya sezon bulunamadı.";
            }

            return RedirectToAction("SeasonUserList", new { seasonId = seasonId });
        }


        public async Task<IActionResult> DeleteSeason(int seasonId)
        {
            Season season = await _context.Seasons.FirstOrDefaultAsync(x => x.Id == seasonId);
            var result = _context.Seasons.Remove(season);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Sezon başarıyla silindi!";
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> UpdateSeason(int seasonId)
        {
            var season = _context.Seasons.FirstOrDefault(x => x.Id == seasonId);

            return View(season);

        }

        [HttpPost]
        public async Task<IActionResult> UpdateSeason(Season model)
        {
            var season = await _context.Seasons.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (season == null)
            {
                return NotFound();
            }



            season.Name = model.Name ?? "";
            season.Date = model.Date;
            season.Place = model.Place;
            season.GameLimit = model.GameLimit;
            season.TimeLimit = model.TimeLimit;
            season.OnePointValue = model.OnePointValue;
            season.CreateDate = DateTime.Now;

            try
            {
                var result = _context.Seasons.Update(season);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Sezon başarıyla güncellendi!";
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);

                return View();
            }
            return View(season);

        }

        public IActionResult SeasonGames(int seasonId)
        {


            var season = _context.Seasons.Include(s => s.Users).FirstOrDefault(s => s.Id == seasonId);

            //var usersForSelectedSeason = season.Users.Select(u => new
            //UserViewModel
            //{
            //    UserId = u.Id,
            //    UserName = u.UserName,
            //    Point = _context.Scores
            //.Where(score => score.AppUserId == u.Id && score.Game.SeasonId == seasonId)
            //.Sum(score => score.Point)
            //}).ToList();

            var usersForSelectedSeason = season.Users.Select(u => new
            PointViewModel
            {
               
                UserId = u.Id,
                UserName = u.UserName,
                Points = _context.Scores
             .Where(score => score.AppUserId == u.Id && score.Game.SeasonId == seasonId)
              .OrderBy(score => score.Game.Id)
              .Select(score => new PointAndGame
              {
                  Point = score.Point,
                  GameNo = score.Game.Id
              })
             .ToList()
            }).ToList();

            var viewModel = new ScoreDetailViewModel
            {


                Season = season,
                Users = usersForSelectedSeason


            };


            //int targetSeasonId = seasonId; // Replace with the desired SeasonId

            //// Query for scores associated with the specified SeasonId
            //var scoresForSeason = _context.Scores
            //    .Where(score => score.Game != null && score.Game.SeasonId == targetSeasonId)
            //    .ToList();

            //// Calculate the total points for the season
            //int totalPointsForSeason = scoresForSeason.Sum(score => score.Point);

            return View(viewModel);
        }

    }
}
