﻿/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;

namespace Quanlythuvien.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class BorrowedsController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public BorrowedsController(QuanlythuvienDbContext context)
        {
            _context = context;
        }

        // GET: Borroweds
        public async Task<IActionResult> Index()
        {
            var quanlythuvienDbContext = _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Libra)
                .Include(b => b.Student);
            return View(await quanlythuvienDbContext.ToListAsync());
        }

        // GET: Borroweds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowed = await _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Libra)
                .Include(b => b.Student)
                .FirstOrDefaultAsync(m => m.BorrowId == id);
            if (borrowed == null)
            {
                return NotFound();
            }

            return View(borrowed);
        }

        // GET: Borroweds/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Title");
            ViewData["LibraId"] = new SelectList(_context.Librarians, "LibraId", "FullName");
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BorrowId,StudentId,BookId,BorrowDate,DueDate,ReturnDate,LibraId,FineAmount,BookStatus,Status")] Borrowed borrowed)
        {
            if (ModelState.IsValid)
            {
                if (!borrowed.BorrowDate.HasValue) borrowed.BorrowDate = DateOnly.FromDateTime(DateTime.Now);
                _context.Add(borrowed);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Title", borrowed.BookId);
            ViewData["LibraId"] = new SelectList(_context.Librarians, "LibraId", "FullName", borrowed.LibraId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName", borrowed.StudentId);
            return View(borrowed);
        }

        // GET: Borroweds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowed = await _context.Borroweds.FindAsync(id);
            if (borrowed == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Title", borrowed.BookId);
            ViewData["LibraId"] = new SelectList(_context.Librarians, "LibraId", "FullName", borrowed.LibraId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName", borrowed.StudentId);
            return View(borrowed);
        }

        // POST: Borroweds/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BorrowId,StudentId,BookId,BorrowDate,DueDate,ReturnDate,LibraId,FineAmount,BookStatus,Status")] Borrowed borrowed)
        {
            if (id != borrowed.BorrowId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(borrowed);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowedExists(borrowed.BorrowId))
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
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Title", borrowed.BookId);
            ViewData["LibraId"] = new SelectList(_context.Librarians, "LibraId", "FullName", borrowed.LibraId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName", borrowed.StudentId);
            return View(borrowed);
        }

        // GET: Borroweds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowed = await _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Libra)
                .Include(b => b.Student)
                .FirstOrDefaultAsync(m => m.BorrowId == id);
            if (borrowed == null)
            {
                return NotFound();
            }

            return View(borrowed);
        }

        // POST: Borroweds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrowed = await _context.Borroweds.FindAsync(id);
            if (borrowed != null)
            {
                _context.Borroweds.Remove(borrowed);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BorrowedExists(int id)
        {
            return _context.Borroweds.Any(e => e.BorrowId == id);
        }

        private bool IsOverdue(DateTime? dueDate)
        {
            return dueDate.HasValue && dueDate.Value < DateTime.Now;
        }
    }
}