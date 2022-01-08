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
    public class RFIDCardsController : Controller
    {
        private readonly DatabaseContext _context;

        public RFIDCardsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: RFIDCards
        public async Task<IActionResult> Index()
        {
            return View(await _context.RFIDCards.ToListAsync());
        }

        // GET: RFIDCards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rFIDCard = await _context.RFIDCards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rFIDCard == null)
            {
                return NotFound();
            }

            return View(rFIDCard);
        }

        // GET: RFIDCards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RFIDCards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CardNumber,IsActive")] RFIDCard rFIDCard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rFIDCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rFIDCard);
        }

        // GET: RFIDCards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rFIDCard = await _context.RFIDCards.FindAsync(id);
            if (rFIDCard == null)
            {
                return NotFound();
            }
            return View(rFIDCard);
        }

        // POST: RFIDCards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CardNumber,IsActive")] RFIDCard rFIDCard)
        {
            if (id != rFIDCard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rFIDCard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RFIDCardExists(rFIDCard.Id))
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
            return View(rFIDCard);
        }

        // GET: RFIDCards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rFIDCard = await _context.RFIDCards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rFIDCard == null)
            {
                return NotFound();
            }

            return View(rFIDCard);
        }

        // POST: RFIDCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rFIDCard = await _context.RFIDCards.FindAsync(id);
            _context.RFIDCards.Remove(rFIDCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RFIDCardExists(int id)
        {
            return _context.RFIDCards.Any(e => e.Id == id);
        }
    }
}
