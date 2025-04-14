using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EventsAndAssignments_DataTransfer.DAO.MIMPublish2Db
{
    public partial class EventsAndAssignmentsContext : DbContext
    {
        public EventsAndAssignmentsContext()
        {
        }

        public EventsAndAssignmentsContext(DbContextOptions<EventsAndAssignmentsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PuplicEmployeeView> PuplicEmployeeViews { get; set; } = null!;
        public virtual DbSet<PuplicOrganizationsView> PuplicOrganizationsViews { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=EventsAndAssignments");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<PuplicEmployeeView>(entity => entity.HasKey(e => e.PositionId));
            //modelBuilder.Entity<PuplicOrganizationsView>(entity => entity.HasKey(e => e.OrganizationId));
            base.OnModelCreating(modelBuilder);
        }
    }
}
