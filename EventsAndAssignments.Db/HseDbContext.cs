using EventsAndAssignments.Services.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EventsAndAssignments.Db
{
    public class HseDbContext : DbContext
    {
        public DbSet<HolidayInfo> HolidayInfo { get; set; }

        public HseDbContext(DbContextOptions<HseDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ValueConverter<bool, string?> freeDayConverter = new(
                v => v ? "X" : null,
                v => v != null && v == "X");

            ValueConverter<bool, string?> holidayConverter = new(
                v => v ? "X" : null,
                v => v != null && v == "X");

            ValueConverter< short, string?> weekdayConverter = new(
                v => (v ==  0) ? null : v.ToString(),
                v => (v == null) ? (short)0 : short.Parse(v));

            modelBuilder.Entity<HolidayInfo>()
                .Property(e => e.FreeDay)
                .HasConversion(freeDayConverter);

            modelBuilder.Entity<HolidayInfo>()
                .Property(e => e.Holiday)
                .HasConversion(holidayConverter);

            modelBuilder.Entity<HolidayInfo>()
                .Property(e => e.Weekday)
                .HasConversion(weekdayConverter);

            modelBuilder.Entity<HolidayInfo>().ToTable("Holidays", "MasterData");
        }
    }
}