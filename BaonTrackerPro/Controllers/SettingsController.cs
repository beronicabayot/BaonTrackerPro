using System.Security.Claims;
using BaonTrackerPro.Data;
using BaonTrackerPro.Models.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BaonTrackerPro.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<BaonTrackerPro.Models.AppUser> _passwordHasher = new();

        public SettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var user = await _context.AppUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login", "Account");

            var username = !string.IsNullOrWhiteSpace(user.Username)
                ? user.Username
                : (user.Email.Contains('@') ? user.Email.Split('@')[0] : user.Email);

            var vm = new SettingsViewModel
            {
                Name = user.Name,
                Email = user.Email,
                Username = username,
                ProfileImageUrl = string.IsNullOrWhiteSpace(user.ProfileIconPath)
                    ? "/Assets/images/busineswoman.png"
                    : user.ProfileIconPath,
                DateOfBirthLabel = user.DateOfBirthUtc.HasValue
                    ? user.DateOfBirthUtc.Value.ToLocalTime().ToString("MMMM d, yyyy")
                    : "—",
                GenderLabel = string.IsNullOrWhiteSpace(user.Gender) ? "—" : user.Gender
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUsername(UpdateUsernameViewModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var userId = GetCurrentUserId();
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login", "Account");

            var username = model.Username.Trim();
            var taken = await _context.AppUsers.AnyAsync(u => u.Id != userId && u.Username != null && u.Username.ToLower() == username.ToLower());
            if (taken)
            {
                TempData["SettingsError"] = "Username is already taken.";
                return RedirectToAction(nameof(Index));
            }

            user.Username = username;
            await _context.SaveChangesAsync();

            // Refresh auth cookie so the sidebar name updates immediately.
            var displayName = !string.IsNullOrWhiteSpace(user.Username) ? user.Username : user.Name;
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, displayName),
                new(ClaimTypes.Email, user.Email)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14)
                });

            TempData["SettingsSuccess"] = "Username updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetBasicInfo(SetBasicInfoViewModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var userId = GetCurrentUserId();
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login", "Account");

            // Once set, cannot be changed.
            if (user.DateOfBirthUtc.HasValue || !string.IsNullOrWhiteSpace(user.Gender))
            {
                TempData["SettingsError"] = "Date of birth and gender can only be set once.";
                return RedirectToAction(nameof(Index));
            }

            user.DateOfBirthUtc = DateTime.SpecifyKind(model.DateOfBirth, DateTimeKind.Local).ToUniversalTime();
            user.Gender = model.Gender.Trim();
            await _context.SaveChangesAsync();

            TempData["SettingsSuccess"] = "Basic info saved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var userId = GetCurrentUserId();
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login", "Account");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword);
            if (result == PasswordVerificationResult.Failed)
            {
                TempData["SettingsError"] = "Current password is incorrect.";
                return RedirectToAction(nameof(Index));
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
            await _context.SaveChangesAsync();

            TempData["SettingsSuccess"] = "Password updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectIcon(string iconPath)
        {
            var userId = GetCurrentUserId();
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login", "Account");

            // Only allow selecting from /Assets/images/
            if (string.IsNullOrWhiteSpace(iconPath) || !iconPath.StartsWith("/Assets/images/", StringComparison.OrdinalIgnoreCase))
            {
                TempData["SettingsError"] = "Invalid icon selection.";
                return RedirectToAction(nameof(Index));
            }

            user.ProfileIconPath = iconPath;
            await _context.SaveChangesAsync();

            TempData["SettingsSuccess"] = "Profile icon updated.";
            return RedirectToAction(nameof(Index));
        }

        private int GetCurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : 0;
        }
    }
}

