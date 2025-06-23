using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EVENT_EASE.Models
{
    public partial class Cldvdb1Context : DbContext
    {
        public Cldvdb1Context()
        {
        }

        public Cldvdb1Context(DbContextOptions<Cldvdb1Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Venue> Venues { get; set; }
        public virtual DbSet<EventType> EventTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            => optionsBuilder.UseSqlServer("Server=huzz.database.windows.net;Database=CLDVdb1;User ID=chuzz;Password=Unbornfidded24;TrustServerCertificate=True;Encrypt=True");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951AED9876C63E");

                entity.Property(e => e.ClientEmail).HasMaxLength(100);
                entity.Property(e => e.ClientName).HasMaxLength(100);

                entity.HasOne(d => d.Event).WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK__Bookings__EventI__60A75C0F");

                entity.HasOne(d => d.Venue)
     .WithMany(v => v.Bookings)  // specify the collection property here
     .HasForeignKey(d => d.VenueId)
     .HasConstraintName("FK_Bookings_Venue");


            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.EventId).HasName("PK__Events__7944C81065D06940");

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasOne(d => d.Venue).WithMany(p => p.Events)
                    .HasForeignKey(d => d.VenueId)
                    .HasConstraintName("FK__Events__VenueId__619B8048");

                entity.HasOne(d => d.EventType).WithMany(p => p.Events)
                    .HasForeignKey(d => d.EventTypeId)
                    .HasConstraintName("FK_Events_EventType");
            });

            modelBuilder.Entity<Venue>(entity =>
            {
                entity.HasKey(e => e.VenueId).HasName("PK__Venues__3C57E5F2102A298A");

                entity.Property(e => e.Location).HasMaxLength(100);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            });

            modelBuilder.Entity<EventType>(entity =>
            {
                entity.ToTable("EventType");
                entity.HasKey(e => e.EventTypeId).HasName("PK_EventType");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.HasMany(e => e.Events)
                    .WithOne(e => e.EventType)
                    .HasForeignKey(e => e.EventTypeId)
                    .HasConstraintName("FK_Events_EventType");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
