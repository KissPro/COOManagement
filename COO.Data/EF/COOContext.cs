using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace COO.Data.EF
{
    public partial class COOContext : DbContext
    {
        public COOContext()
        {
        }

        public COOContext(DbContextOptions<COOContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblBoom> TblBoom { get; set; }
        public virtual DbSet<TblConfig> TblConfig { get; set; }
        public virtual DbSet<TblCountryShip> TblCountryShip { get; set; }
        public virtual DbSet<TblDeliverySales> TblDeliverySales { get; set; }
        public virtual DbSet<TblDsmanual> TblDsmanual { get; set; }
        public virtual DbSet<TblEcusTs> TblEcusTs { get; set; }
        public virtual DbSet<TblPlant> TblPlant { get; set; }


        // For migrations database
//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("data source=HVNN0606\\SQLEXPRESS;initial catalog=COO;user id=sa;password=123;MultipleActiveResultSets=True;");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblBoom>(entity =>
            {
                entity.HasKey(e => e.Material)
                    .HasName("PK_tblBoom_1");

                entity.ToTable("tbl_Boom");

                entity.Property(e => e.Material)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.ParentMaterial)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SortString)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TblConfig>(entity =>
            {
                entity.ToTable("tbl_Config");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Dsruntime)
                    .IsRequired()
                    .HasColumnName("DSRuntime")
                    .HasMaxLength(50);

                entity.Property(e => e.DstimeLastMonth).HasColumnName("DSTimeLastMonth");

                entity.Property(e => e.DstimeLastYear).HasColumnName("DSTimeLastYear");

                entity.Property(e => e.DstimeNextMonth).HasColumnName("DSTimeNextMonth");

                entity.Property(e => e.DstimeNextYear).HasColumnName("DSTimeNextYear");

                entity.Property(e => e.EcusRuntime)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RemarkConfig).HasMaxLength(500);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TblCountryShip>(entity =>
            {
                entity.ToTable("tbl_CountryShip");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RemarkCountry).HasMaxLength(500);

                entity.Property(e => e.ShipId)
                    .IsRequired()
                    .HasColumnName("ShipID")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TblDeliverySales>(entity =>
            {
                entity.ToTable("tbl_DeliverySales");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActualGidate)
                    .HasColumnName("ActualGIDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Dnqty).HasColumnName("DNQty");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.MaterialDesc)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.MaterialParent)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NetPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NetValue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PartyName)
                    .HasMaxLength(200);

                entity.Property(e => e.HarmonizationCode)
                    .HasMaxLength(200);
                entity.Property(e => e.Address)
                    .HasMaxLength(500);

                entity.Property(e => e.PlanGidate)
                    .HasColumnName("PlanGIDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.PlanGisysDate)
                    .HasColumnName("PlanGISysDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.SaleUnit)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ShipToCountry)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TblDsmanual>(entity =>
            {
                entity.ToTable("tbl_DSManual");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Cooform)
                    .HasColumnName("COOForm")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Coono).HasColumnName("COONo");

                entity.Property(e => e.CourierDate).HasColumnType("datetime");

                entity.Property(e => e.DeliverySalesId).HasColumnName("DeliverySalesID");

                entity.Property(e => e.Origin)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ReceiptDate).HasColumnType("datetime");

                entity.Property(e => e.RemarkDs)
                    .HasColumnName("RemarkDS")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.ReturnDate).HasColumnType("datetime");

                entity.Property(e => e.TrackingDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.DeliverySales)
                    .WithMany(p => p.TblDsmanual)
                    .HasForeignKey(d => d.DeliverySalesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblDSManual_tblDeliverySales");
            });

            modelBuilder.Entity<TblEcusTs>(entity =>
            {
                entity.ToTable("tbl_EcusTS");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Country)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DonGiaHd)
                    .HasColumnName("DonGiaHD")
                    .HasColumnType("decimal(18, 4)");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.MaHang)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaHs)
                    .HasColumnName("MaHS")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NgayDk)
                    .HasColumnName("NgayDK")
                    .HasColumnType("datetime");

                entity.Property(e => e.SoTk)
                    .IsRequired()
                    .HasColumnName("SoTK")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenHang).HasMaxLength(500);
            });

            modelBuilder.Entity<TblPlant>(entity =>
            {
                entity.ToTable("tbl_Plant");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.RemarkCountry).HasMaxLength(500);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
