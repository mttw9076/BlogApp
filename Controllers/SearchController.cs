using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public SearchController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Users(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return View(new List<IdentityUser>());

            var users = await _userManager.Users
                .Where(u => u.UserName.Contains(q))
                .ToListAsync();

            return View(users);
        }
    }
}
