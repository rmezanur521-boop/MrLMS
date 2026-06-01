using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MrLMS.Data;
using MrLMS.Helper;
using MrLMS.Models;
using MrLMS.Models.ViewModels;

namespace MrLMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db)
        {
            _db = db;
        }

        // ─────────────────────────────────────────
        // GET: /Account/Login
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var member = await _db.Members
                .FirstOrDefaultAsync(m => m.Email == model.Email && m.IsActive == true);

            if (member == null || member.Salt == null || member.PasswordHash == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            bool isValid = PasswordHelper.VerifyPassword(model.Password, member.Salt.Value, member.PasswordHash);
            if (!isValid)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // Get role
            var memberRole = await _db.MemberRoles
                .Include(mr => mr.Role)
                .FirstOrDefaultAsync(mr => mr.MemberId == member.Id && mr.IsActive == true);

            string roleName = memberRole?.Role?.RoleName ?? "Member";

            // Set Session
            var sessionData = new SessionData
            {
                MemberId = member.Id,
                Name = member.Name ?? "",
                Email = member.Email ?? "",
                Role = roleName
            };

            SessionHelper.SetObject(HttpContext.Session, "UserSession", sessionData);
            HttpContext.Session.SetString("UserEmail", member.Email ?? "");
            HttpContext.Session.SetString("UserRole", roleName);
            HttpContext.Session.SetString("UserName", member.Name ?? "");
            HttpContext.Session.SetInt32("UserId", member.Id);

            // ── Redirect: returnUrl takes priority ──────────────────
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return roleName switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Librarian" => RedirectToAction("Dashboard", "Librarian"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // ─────────────────────────────────────────
        // GET: /Account/Register
        // ─────────────────────────────────────────
        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // ─────────────────────────────────────────
        // POST: /Account/Register
        // ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool emailExists = await _db.Members.AnyAsync(m => m.Email == model.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            var salt = Guid.NewGuid();
            var hash = PasswordHelper.HashPassword(model.Password, salt);

            var member = new Member
            {
                Name = model.Name,
                Email = model.Email,
                Mobile = model.Mobile,
                PasswordHash = hash,
                Salt = salt,
                IsActive = true,
                CreatedOn = DateTime.Now
            };

            _db.Members.Add(member);
            await _db.SaveChangesAsync();

            // Assign "Member" role automatically
            var memberRole = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == "Member");
            if (memberRole != null)
            {
                _db.MemberRoles.Add(new MemberRole
                {
                    MemberId = member.Id,
                    RoleId = memberRole.Id,
                    IsActive = true
                });
                await _db.SaveChangesAsync();
            }

            TempData["Success"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }

        // ─────────────────────────────────────────
        // GET: /Account/Logout
        // ─────────────────────────────────────────
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ─────────────────────────────────────────
        // GET: /Account/AccessDenied
        // ─────────────────────────────────────────
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}