using System;
using System.Collections.Generic;
using System.Text;
using PGManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PGManager.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Facility> Facilities { get; set; }

        public DbSet<AvailableFacility> AvailableFacilities { get; set; }

        public DbSet<PG> PG { get; set; }

        public DbSet<Photo> Photos { get; set; }

        public DbSet<PriceTier> PriceTiers { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<Bed> Beds { get; set; }

        public DbSet<UserDocument> Documents { get; set; }

        public DbSet<PGRequest> PGRequests { get; set; }

        public DbSet<Stay> Stays { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PriceTier>()
                .HasMany(pt => pt.Rooms)
                .WithOne(r=>r.PriceTier)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Room>()
                .HasMany(room => room.Beds)
                .WithOne(Bed => Bed.Room)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PGRequest>()
                .HasOne(req => req.PriceTier)
                .WithMany(pt => pt.PGRequests)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stay>()
                .HasOne(s => s.PG)
                .WithMany(pg => pg.Stays)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stay>()
                .HasOne(s => s.Room)
                .WithMany(room => room.Stays)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stay>()
                .HasOne(s => s.Bed)
                .WithMany(bed => bed.Stays)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
