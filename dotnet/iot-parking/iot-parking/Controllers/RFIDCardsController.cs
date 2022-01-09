using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index(string? search)
        {
            var databaseContext = _context.RFIDCards.Include(c => c.CardOwner).ToList();
            if (search != null)
            {
                var filteredList = databaseContext.FindAll(
                    c => { 
                        if(c.CardOwner != null)
                            return c.CardNumber == search || c.CardOwner.Firstname == search || c.CardOwner.Lastname == search || c.CardOwner.Email == search;
                        return c.CardNumber == search;
                    });
                return View(filteredList);
            }

            return View(databaseContext);
        }

        // GET: RFIDCards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rFIDCard = await _context.RFIDCards
                .Include(c => c.CardOwner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rFIDCard == null)
            {
                return NotFound();
            }

            return View(rFIDCard);
        }

        // GET: RFIDCards/Create
        public async Task<IActionResult> Create(int? cardId)
        {
            TempData["pickCard"] = "Create";
            if (cardId != null)
            {
                var scannedCard = await _context.ScannedCards.FirstOrDefaultAsync(c => c.Id == cardId);

                if(scannedCard != null)
                {
                    ViewBag.cardNr = scannedCard.CardNumber;
                }

            }
            return View();
        }

        public async Task<IActionResult> PickCard()
        {
            var cards = await _context.ScannedCards.ToListAsync();

            return View(cards);
        }

        // POST: RFIDCards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CardNumber,IsActive,HasOwner,Firstname,Lastname,Email,IssueDate,ValidDate")] CardOwnerRFIDCard card)
        {
            if (ModelState.IsValid)
            {
                RFIDCard rFIDCard = new();
                rFIDCard.CardNumber = card.CardNumber;
                rFIDCard.IsActive = card.IsActive;

                _context.Add(rFIDCard);
                _context.SaveChanges();

                if (card.HasOwner) {
                    var newCard = _context.RFIDCards.FirstOrDefault(c => c.CardNumber == card.CardNumber);
                    CardOwner cardOwner = new();
                    cardOwner.Firstname = card.Firstname;
                    cardOwner.Lastname = card.Lastname;
                    cardOwner.Email = card.Email;
                    cardOwner.IssueDate = card.IssueDate.Value;
                    cardOwner.ValidDate = card.ValidDate.Value;
                    cardOwner.CardId = newCard.Id;

                    _context.Add(cardOwner);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(card);
        }

        // GET: RFIDCards/Edit/5
        public async Task<IActionResult> Edit(int? id, int? cardId)
        {
            if (id == null)
            {
                return NotFound();
            }

            TempData["pickCard"] = "Edit";
            TempData["carId"] = id;

            var rFIDCard = await _context.RFIDCards
                .Include(c => c.CardOwner)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rFIDCard == null)
            {
                return NotFound();
            }

            if (cardId != null)
            {
                var scannedCard = await _context.ScannedCards.FirstOrDefaultAsync(c => c.Id == cardId);

                if (scannedCard != null)
                {
                    ViewBag.cardNr = scannedCard.CardNumber;
                }

            }
            else
            {
                ViewBag.cardNr = rFIDCard.CardNumber;
            }

            CardOwnerRFIDCard card = new();
            card.CardNumber = rFIDCard.CardNumber;
            card.IsActive = rFIDCard.IsActive;
            
            if(rFIDCard.CardOwner != null)
            {
                card.HasOwner = true;
                card.Firstname = rFIDCard.CardOwner.Firstname;
                card.Lastname = rFIDCard.CardOwner.Lastname;
                card.Email = rFIDCard.CardOwner.Email;
                card.IssueDate = rFIDCard.CardOwner.IssueDate;
                card.ValidDate = rFIDCard.CardOwner.ValidDate;
            }
            else
            {
                card.HasOwner = false;
            }

            return View(card);
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
                .Include(c => c.CardOwner)
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
