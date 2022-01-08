using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using iot_parking.Database;
using iot_parking.Models;

namespace iot_parking.Controllers
{
    public class CardOwnersController : Controller
    {
        private readonly DatabaseContext _context;

        public CardOwnersController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: CardOwners
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.CardOwners.Include(c => c.RFIDCard);
            return View(await databaseContext.ToListAsync());
        }

        // GET: CardOwners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cardOwner = await _context.CardOwners
                .Include(c => c.RFIDCard)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cardOwner == null)
            {
                return NotFound();
            }

            return View(cardOwner);
        }

        // GET: CardOwners/Create
        public IActionResult Create()
        {
            ViewData["CardId"] = new SelectList(_context.RFIDCards, "Id", "CardNumber");
            return View();
        }

        // POST: CardOwners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Firstname,Lastname,Email,IssueDate,ValidDate,CardId")] CardOwner cardOwner)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cardOwner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CardId"] = new SelectList(_context.RFIDCards, "Id", "CardNumber", cardOwner.CardId);
            return View(cardOwner);
        }

        // GET: CardOwners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cardOwner = await _context.CardOwners.FindAsync(id);
            if (cardOwner == null)
            {
                return NotFound();
            }
            ViewData["CardId"] = new SelectList(_context.RFIDCards, "Id", "CardNumber", cardOwner.CardId);
            return View(cardOwner);
        }

        // POST: CardOwners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Firstname,Lastname,Email,IssueDate,ValidDate,CardId")] CardOwner cardOwner)
        {
            if (id != cardOwner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cardOwner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CardOwnerExists(cardOwner.Id))
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
            ViewData["CardId"] = new SelectList(_context.RFIDCards, "Id", "CardNumber", cardOwner.CardId);
            return View(cardOwner);
        }

        // GET: CardOwners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cardOwner = await _context.CardOwners
                .Include(c => c.RFIDCard)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cardOwner == null)
            {
                return NotFound();
            }

            return View(cardOwner);
        }

        // POST: CardOwners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cardOwner = await _context.CardOwners.FindAsync(id);
            _context.CardOwners.Remove(cardOwner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CardOwnerExists(int id)
        {
            return _context.CardOwners.Any(e => e.Id == id);
        }
    }
}
