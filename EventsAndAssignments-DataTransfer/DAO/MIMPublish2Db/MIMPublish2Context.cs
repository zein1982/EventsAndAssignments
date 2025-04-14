using Microsoft.EntityFrameworkCore;

namespace EventsAndAssignments_DataTransfer.DAO.MIMPublish2Db
{
    public partial class MIMPublish2Context : DbContext
    {
        private readonly string _defaultConnectionString = "Data Source=MSK-SCL-003.msk.evraz.com;Initial Catalog=MIMPublish2;Integrated Security=True;";

        public virtual DbSet<PuplicEmployeeView> PuplicEmployeeViews { get; set; } = null!;
        public virtual DbSet<PuplicOrganizationsView> PuplicOrganizationsViews { get; set; } = null!;

        public MIMPublish2Context()
        {
        }

        public MIMPublish2Context(DbContextOptions<MIMPublish2Context> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_defaultConnectionString);
                //optionsBuilder.UseSqlServer("name=MIMPublish2");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<PuplicEmployeeView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("PuplicEmployeeView", "publicViewSchema");

                entity.Property(e => e.AnyLastModfication).HasColumnType("datetime");

                entity.Property(e => e.DepartmentCode).HasMaxLength(448);

                entity.Property(e => e.DepartmentLastModfication).HasColumnType("datetime");

                entity.Property(e => e.DepartmentName).HasMaxLength(448);

                entity.Property(e => e.Domain).HasMaxLength(448);

                entity.Property(e => e.Email).HasMaxLength(448);

                entity.Property(e => e.EndDate).HasMaxLength(448);

                entity.Property(e => e.FirstName).HasMaxLength(448);

                entity.Property(e => e.HireDate).HasMaxLength(448);

                entity.Property(e => e.IsSfrelevant)
                    .HasMaxLength(448)
                    .HasColumnName("IsSFRelevant");

                entity.Property(e => e.LastName).HasMaxLength(448);

                entity.Property(e => e.Login).HasMaxLength(448);

                entity.Property(e => e.MiddleName).HasMaxLength(448);

                entity.Property(e => e.Occupation).HasMaxLength(448);

                entity.Property(e => e.OrganizationCode).HasMaxLength(448);

                entity.Property(e => e.OrganizationLastModfication).HasColumnType("datetime");

                entity.Property(e => e.OrganizationName).HasMaxLength(448);

                entity.Property(e => e.PersonLastModfication).HasColumnType("datetime");

                entity.Property(e => e.PositionCode).HasMaxLength(448);

                entity.Property(e => e.PositionLastModfication).HasColumnType("datetime");

                entity.Property(e => e.PositionName).HasMaxLength(448);

                entity.Property(e => e.TabelNumber).HasMaxLength(448);
            });

            modelBuilder.Entity<PuplicOrganizationsView>(entity =>
            {
                entity.HasKey(e => e.OrganizationId);
                entity.ToView("PuplicOrganizationsView", "publicViewSchema");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}