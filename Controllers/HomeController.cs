using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Jeśli użytkownik nie jest zalogowany → strona publiczna
            if (!User.Identity.IsAuthenticated)
                return View("IndexPublic");

            var userId = _userManager.GetUserId(User);

            // Pobierz listę subskrybowanych użytkowników
            var subscribedUsers = await _context.Subscriptions
                .Where(s => s.SubscriberId == userId)
                .Select(s => s.TargetUserId)
                .ToListAsync();

            // Pobierz posty tylko od subskrybowanych użytkowników
            var posts = await _context.Posts
                .Where(p => subscribedUsers.Contains(p.AuthorId))
                .OrderByDescending(p => p.CreatedAt)
                .Take(20)
                .ToListAsync();

            return View("IndexLogged", posts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
