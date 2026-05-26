using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // PROFIL PRYWATNY
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new UserProfileViewModel
            {
                User = user,
                PostsCount = await _context.Posts.CountAsync(p => p.AuthorId == user.Id),

                FriendsCount = await _context.Friends
                    .CountAsync(f =>
                        f.Status == FriendStatus.Accepted &&
                        (f.RequesterId == user.Id || f.ReceiverId == user.Id)),

                SubscriptionsCount = await _context.Subscriptions
                    .CountAsync(s => s.SubscriberId == user.Id)
            };

            return View(model);
        }

        // EDYCJA PROFILU (GET)
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        // EDYCJA PROFILU (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser model, IFormFile? profileImage)
        {
            var user = await _userManager.GetUserAsync(User);

            user.DisplayName = model.DisplayName;
            user.Bio = model.Bio;

            // ZAPIS ZDJĘCIA PROFILOWEGO
            if (profileImage != null && profileImage.Length > 0)
            {
                var folder = "wwwroot/profile-images";
                Directory.CreateDirectory(folder);

                var fileName = $"{user.Id}_{Path.GetFileName(profileImage.FileName)}";
                var filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                user.ProfileImagePath = "/profile-images/" + fileName;
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Profile");
        }

        // PUBLICZNY PROFIL
        public async Task<IActionResult> View(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var model = new UserProfileViewModel
            {
                User = user,

                // LICZBA POSTÓW
                PostsCount = await _context.Posts
                    .CountAsync(p => p.AuthorId == user.Id),

                // LICZBA ZNAJOMYCH (zaakceptowane relacje)
                FriendsCount = await _context.Friends
                    .CountAsync(f =>
                        f.Status == FriendStatus.Accepted &&
                        (f.RequesterId == user.Id || f.ReceiverId == user.Id)
                    ),

                // LICZBA OBSERWUJĄCYCH (subskrypcji)
                SubscriptionsCount = await _context.Subscriptions
                    .CountAsync(s => s.TargetUserId == user.Id)
            };

            return View(model);
        }

    }
}
