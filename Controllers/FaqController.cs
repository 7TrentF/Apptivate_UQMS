using Apptivate_UQMS_WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Apptivate_UQMS_WebApp.Models.KnowledgeBaseFAQ;
using System;

using Microsoft.EntityFrameworkCore;

namespace Apptivate_UQMS_WebApp.Controllers
{
    public class FAQController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FAQController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin Actions
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            var faqs = _context.Faq.ToList();
            return View(faqs);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(FAQ faq)
        {
            if (ModelState.IsValid)
            {
                faq.CreatedDate = DateTime.Now;
                faq.LastUpdatedDate = DateTime.Now;
                _context.Add(faq);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Admin));
            }
            return View(faq);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var faq = await _context.Faq.FindAsync(id);
            if (faq == null)
                return NotFound();

            return View(faq);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, FAQ faq)
        {
            if (id != faq.FaqId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    faq.LastUpdatedDate = DateTime.Now;
                    _context.Update(faq);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Faq.Any(e => e.FaqId == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Admin));
            }
            return View(faq);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var faq = await _context.Faq.FindAsync(id);
            if (faq != null)
            {
                _context.Faq.Remove(faq);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Admin));
        }





        // Student Actions
        public async Task<IActionResult> Index()
        {
            var faqs = await _context.Faq.ToListAsync();
            return View(faqs);
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            var faqs = await _context.Faq
                .Where(f => f.Question.Contains(searchTerm) ||
                           f.Answer.Contains(searchTerm) ||
                           f.Category.Contains(searchTerm))
                .ToListAsync();
            return View("Index", faqs);
        }
    }
}
