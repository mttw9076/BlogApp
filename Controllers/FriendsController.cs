using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FriendsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // LISTA ZNAJOMYCH
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var friends = await _context.Friends
                .Where(f =>
                    (f.RequesterId == userId || f.ReceiverId == userId) &&
                    f.Status == FriendStatus.Accepted)
                .ToListAsync();

            return View(friends);
        }

        // WYSŁANIE ZAPROSZENIA
        [HttpPost]
        public async Task<IActionResult> SendRequest(string userId)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == userId)
                return BadRequest("Nie możesz dodać siebie.");

            var exists = await _context.Friends.AnyAsync(f =>
                (f.RequesterId == currentUserId && f.ReceiverId == userId) ||
                (f.RequesterId == userId && f.ReceiverId == currentUserId));

            if (exists)
                return BadRequest("Relacja już istnieje.");

            var request = new Friend
            {
                RequesterId = currentUserId,
                ReceiverId = userId,
                Status = FriendStatus.Pending
            };

            _context.Friends.Add(request);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // AKCEPTOWANIE ZAPROSZENIA
        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            var userId = _userManager.GetUserId(User);
            var request = await _context.Friends.FindAsync(id);

            if (request == null || request.ReceiverId != userId)
                return NotFound();

            request.Status = FriendStatus.Accepted;
            request.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // ODRZUCANIE ZAPROSZENIA
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var userId = _userManager.GetUserId(User);
            var request = await _context.Friends.FindAsync(id);

            if (request == null || request.ReceiverId != userId)
                return NotFound();

            request.Status = FriendStatus.Rejected;
            request.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // USUWANIE ZNAJOMEGO
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = _userManager.GetUserId(User);
            var relation = await _context.Friends.FindAsync(id);

            if (relation == null ||
                (relation.RequesterId != userId && relation.ReceiverId != userId))
                return NotFound();

            _context.Friends.Remove(relation);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Search(string q)
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
