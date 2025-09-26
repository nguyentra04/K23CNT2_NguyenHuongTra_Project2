using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanlythuvienDB.Models;

namespace QuanlythuvienDB.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public UserRolesController(QuanlythuvienDbContext context)
        {
            _context = context;
        }

        // GET: UserRoles
        public async Task<IActionResult> Index()
        {
            var quanlythuvienDbContext = _context.UserRoles.Include(u => u.Admin).Include(u => u.Libra).Include(u => u.Role).Include(u => u.Student);
            return View(await quanlythuvienDbContext.ToListAsync());
        }

        // GET: UserRoles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userRole = await _context.UserRoles
                .Include(u => u.Admin)
                .Include(u => u.Libra)
                .Include(u => u.Role)
                .Include(u => u.Student)
                .FirstOrDefaultAsync(m => m.UserRoleId == id);
            if (userRole == null)
            {
                return NotFound();
            }

            return View(userRole);
        }

        // GET: UserRoles/Create
        public IActionResult Create()
        {
            ViewData["AdminId"] = new SelectList(_context.Admins, "AdminId", "AdminId");
            ViewData["LibraId"] = new SelectList(_context.Librarians, "LibraId", "LibraId");
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId");
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId");
            return View();
        }

        // POST: UserRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserRoleId,RoleId,AdminId,LibraId,StudentId")] UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminId"] = new SelectList(_context.Admins, "AdminId", "AdminId", userRole.AdminId);
            ViewData["LibraId"] = new SelectList(_context.Librarians, "LibraId", "LibraId", userRole.LibraId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", userRole.RoleId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", userRole.StudentId);
            return View(userRole);
        }

        // GET: UserRoles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
            {
                return NotFound();
            }
            ViewData["AdminId"] = new SelectList(_context.Admins, "AdminId", "AdminId", userRole.AdminId);
            ViewData["LibraId"] = new SelectList(_context.Librarians, "LibraId", "LibraId", userRole.LibraId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", userRole.RoleId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", userRole.StudentId);
            return View(userRole);
        }

        // POST: UserRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserRoleId,RoleId,AdminId,LibraId,StudentId")] UserRole userRole)
        {
            if (id != userRole.UserRoleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserRoleExists(userRole.UserRoleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminId"] = new SelectList(_context.Admins, "AdminId", "AdminId", userRole.AdminId);
            ViewData["LibraId"] = new SelectList(_context.Librarians, "LibraId", "LibraId", userRole.LibraId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", userRole.RoleId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", userRole.StudentId);
            return View(userRole);
        }

        // GET: UserRoles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userRole = await _context.UserRoles
                .Include(u => u.Admin)
                .Include(u => u.Libra)
                .Include(u => u.Role)
                .Include(u => u.Student)
                .FirstOrDefaultAsync(m => m.UserRoleId == id);
            if (userRole == null)
            {
                return NotFound();
            }

            return View(userRole);
        }

        // POST: UserRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserRoleExists(int id)
        {
            return _context.UserRoles.Any(e => e.UserRoleId == id);
        }
    }
}
