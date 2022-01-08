using iot_parking.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iot_parking.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RFIDCard>()
                .HasOne(s => s.CardOwner)
                .WithOne(s => s.RFIDCard)
                .HasForeignKey<CardOwner>(s => s.CardId);
        }

        public DbSet<RFIDCard> RFIDCards { get; set; }

        public DbSet<CardOwner> CardOwners { get; set; }

        public DbSet<Parking> Parkings { get; set; }

        public DbSet<ScannedCard> ScannedCards { get; set; }

        public DbSet<Terminal> Terminals { get; set; }

        private bool CheckCard(RFIDCard? card)
        {
            var parking = card.Parkings.FirstOrDefault(p => p.ExitDate == null);

            if (card == null || parking != null)
                return false;
            else
                return card.IsActive;
        }

        private bool CheckParking(RFIDCard? card)
        {
            var parkings = card.Parkings.Where(p => p.ExitDate == null).ToList();

            if (card == null || parkings.Count != 1 )
                return false;
            else
                return true;
        }

        public async Task<bool> CheckEntry(string terminalNumber, string cardNumber)
        {
            var terminal = Terminals.FirstOrDefault(t => t.TerminalNumber == terminalNumber);

            if (terminal != null & terminal.Type == TerminalTypes.EntryGate)
                return await SaveEntry(cardNumber);
            else
                return false;
            
        }

        public async Task<bool> SaveEntry(string cardNumber)
        {
            var card = RFIDCards.Include(c => c.CardOwner).FirstOrDefault(c => c.CardNumber == cardNumber);

            if (CheckCard(card))
            {
                Parking parking = new Parking()
                {
                    EntryDate = DateTime.Now,
                    CardId = card.Id
                };

                Add(parking);
                await SaveChangesAsync();

                return true;
            }
            else
                return false;
        }

        public async Task<bool> CheckLeave(string terminalNumber, string cardNumber)
        {
            var terminal = Terminals.FirstOrDefault(t => t.TerminalNumber == terminalNumber);

            if (terminal != null & terminal.Type == TerminalTypes.ExitGate)
                return await SaveLeave(cardNumber);
            else
                return false;
        }

        public async Task<bool> SaveLeave(string cardNumber)
        {
            var card = RFIDCards.Include(c => c.CardOwner).FirstOrDefault(c => c.CardNumber == cardNumber);
            var parking = card.Parkings.FirstOrDefault(p => p.ExitDate == null);

            if (CheckParking(card))
            {
                parking.ExitDate = DateTime.Now;
                Update(parking);
                await SaveChangesAsync();

                return true;
            }
            else
                return false;
        }
    }
}
