﻿using iot_parking.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace iot_parking.Database
{
    public enum DbResponse
    {
        Success,
        NotExistingTerminal,
        WrongTerminaltype,
        NotExistingCard,
        DeactivatedCard,
        CardUsedToEntry,
        NotExistingParking
    }

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

        private DbResponse CheckCard(RFIDCard? card)
        {
            if (card == null)
                return DbResponse.NotExistingCard;

            var parking = card.Parkings.FirstOrDefault(p => p.ExitDate == null);

            if (parking != null)
                return DbResponse.CardUsedToEntry;
            else
            {
                if (card.IsActive)
                    return DbResponse.Success;
                else
                    return DbResponse.DeactivatedCard;
            }
        }

        private DbResponse CheckParking(RFIDCard? card)
        {
            if (card == null)
                return DbResponse.NotExistingCard;

            var parkings = card.Parkings.Where(p => p.ExitDate == null).ToList();

            if (parkings.Count != 1)
                return DbResponse.NotExistingParking;
            else
                return DbResponse.Success;
        }

        public async Task<DbResponse> CheckEntry(string terminalNumber, string cardNumber)
        {
            var terminal = Terminals.FirstOrDefault(t => t.TerminalNumber.Equals(terminalNumber));

            if (terminal == null)
                return DbResponse.NotExistingTerminal;
            else if (terminal.Type != TerminalTypes.EntryGate)
                return DbResponse.WrongTerminaltype;
            else
                return await SaveEntry(cardNumber);
            
        }

        private async Task<DbResponse> SaveEntry(string cardNumber)
        {
            var card = RFIDCards.Include(c => c.Parkings).FirstOrDefault(c => c.CardNumber.Equals(cardNumber));
            DbResponse response = CheckCard(card);

            if (card != null && response == DbResponse.Success)
            {
                Parking parking = new Parking()
                {
                    EntryDate = DateTime.Now,
                    CardId = card.Id
                };

                Add(parking);
                await SaveChangesAsync();

                return DbResponse.Success;
            }
            else
                return response;
        }

        public async Task<DbResponse> CheckLeave(string terminalNumber, string cardNumber)
        {
            var terminal = Terminals.FirstOrDefault(t => t.TerminalNumber.Equals(terminalNumber));

            if (terminal == null)
                return DbResponse.NotExistingTerminal;
            else if (terminal.Type != TerminalTypes.ExitGate)
                return DbResponse.WrongTerminaltype;
            else
                return await SaveLeave(cardNumber);
        }

        private async Task<DbResponse> SaveLeave(string cardNumber)
        {
            var card = RFIDCards.Include(c => c.Parkings).FirstOrDefault(c => c.CardNumber.Equals(cardNumber));
            DbResponse response = CheckParking(card);

            if (card != null && response == DbResponse.Success)
            {
                var parking = card.Parkings.FirstOrDefault(p => p.ExitDate == null);
                parking.ExitDate = DateTime.Now;
                Update(parking);
                await SaveChangesAsync();

                return DbResponse.Success;
            }
            else
                return response;
        }
    }
}
