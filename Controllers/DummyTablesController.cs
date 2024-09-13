using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace Apptivate_UQMS_WebApp.Controllers
{
   
    public class DummyTablesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DummyTablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DummyTables
        public async Task<IActionResult> Index()
        {
            return View(await _context.DummyTables.ToListAsync());
        }

        // GET: DummyTables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dummyTable = await _context.DummyTables
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dummyTable == null)
            {
                return NotFound();
            }

            return View(dummyTable);
        }

        // GET: DummyTables/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DummyTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] DummyTable dummyTable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dummyTable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dummyTable);
        }

        // GET: DummyTables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dummyTable = await _context.DummyTables.FindAsync(id);
            if (dummyTable == null)
            {
                return NotFound();
            }
            return View(dummyTable);
        }

        // POST: DummyTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] DummyTable dummyTable)
        {
            if (id != dummyTable.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dummyTable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DummyTableExists(dummyTable.Id))
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
            return View(dummyTable);
        }

        // GET: DummyTables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dummyTable = await _context.DummyTables
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dummyTable == null)
            {
                return NotFound();
            }

            return View(dummyTable);
        }

        // POST: DummyTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dummyTable = await _context.DummyTables.FindAsync(id);
            if (dummyTable != null)
            {
                _context.DummyTables.Remove(dummyTable);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DummyTableExists(int id)
        {
            return _context.DummyTables.Any(e => e.Id == id);
        }
    }
}
