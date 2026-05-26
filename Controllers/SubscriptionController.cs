using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscriptionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // LISTA SUBSKRYPCJI
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var subs = await _context.Subscriptions
                .Where(s => s.SubscriberId == userId)
                .Include(s => s.TargetUser)
                .ToListAsync();

            return View(subs);
        }


        // SUBSKRYBUJ
        [HttpPost]
        public async Task<IActionResult> Subscribe(string userId)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == userId)
                return BadRequest("Nie możesz subskrybować siebie.");

            var exists = await _context.Subscriptions.AnyAsync(s =>
                s.SubscriberId == currentUserId && s.TargetUserId == userId);

            if (exists)
                return BadRequest("Już subskrybujesz tego użytkownika.");

            var sub = new Subscription
            {
                SubscriberId = currentUserId,
                TargetUserId = userId
            };

            _context.Subscriptions.Add(sub);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ODSUBSKRYBUJ
        [HttpPost]
        public async Task<IActionResult> Unsubscribe(int id)
        {
            var userId = _userManager.GetUserId(User);
            var sub = await _context.Subscriptions.FindAsync(id);

            if (sub == null || sub.SubscriberId != userId)
                return NotFound();

            _context.Subscriptions.Remove(sub);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
