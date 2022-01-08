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

    }
}
