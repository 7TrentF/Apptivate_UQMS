using Apptivate_UQMS_WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Apptivate_UQMS_WebApp.Models.KnowledgeBaseFAQ;

namespace Apptivate_UQMS_WebApp.Controllers
{
    public class KnowledgeBaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KnowledgeBaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin Actions
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            var articles = _context.knowledgeBase.ToList();
            return View(articles);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // Admin create/edit with URL validation
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(KnowledgeBase guide)
        {
            if (ModelState.IsValid)
            {
                // Validate that the GitHub Pages URL is accessible
                try
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(guide.GuideUrl);
                        if (!response.IsSuccessStatusCode)
                        {
                            ModelState.AddModelError("GuideUrl", "The guide URL is not accessible. Please verify the URL and ensure the page is published.");
                            return View(guide);
                        }
                    }
                }
                catch
                {
                    ModelState.AddModelError("GuideUrl", "Failed to validate the guide URL. Please verify the URL is correct.");
                    return View(guide);
                }

                guide.CreatedDate = DateTime.Now;
                guide.LastUpdatedDate = DateTime.Now;
                _context.Add(guide);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Admin));
            }
            return View(guide);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var article = await _context.knowledgeBase.FindAsync(id);
            if (article == null)
                return NotFound();

            return View(article);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, KnowledgeBase article)
        {
            if (id != article.ArticleID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    article.LastUpdatedDate = DateTime.Now;
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.knowledgeBase.Any(e => e.ArticleID == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Admin));
            }
            return View(article);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _context.knowledgeBase.FindAsync(id);
            if (article != null)
            {
                _context.knowledgeBase.Remove(article);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Admin));
        }

        // Student Actions
        public async Task<IActionResult> Index()
        {
            var guides = await _context.knowledgeBase
                .OrderBy(k => k.Category)
                .ThenBy(k => k.Title)
                .ToListAsync();

            return View(guides);
        }
        public async Task<IActionResult> Search(string searchTerm)
        {
            var articles = await _context.knowledgeBase
                .Where(a => a.Title.Contains(searchTerm) ||
                           a.Content.Contains(searchTerm) ||
                           a.Category.Contains(searchTerm))
                .ToListAsync();
            return View("Index", articles);
        }
    }
}
