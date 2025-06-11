using AuLacChamCong.Models.Shared;
using AuLacChamCong.Services.Modules.BenhVien.Model;
using AuLacChamCong.Services.Modules.ChamCong.Model;
using AuLacChamCong.Services.Modules.ChamCong.Model.FaceId;
using AuLacChamCong.Services.Modules.DonCongTac.Model;
using AuLacChamCong.Services.Modules.LichLamViec.Model;
using AuLacChamCong.Services.Modules.LichNghi.Model;
using AuLacChamCong.Services.Modules.Notification.Model;
using AuLacChamCong.Services.Modules.Profile.Model;
using AuLacChamCong.Services.Modules.ThongKe.Model;
using AuLacChamCong.Services.Modules.User.Model;
using Microsoft.EntityFrameworkCore;

namespace AuLacChamCong.DataApi
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ChamCongHeader1> ChamCongHeader1s { get; set; }
        public DbSet<ChamCongHeaderDb> ChamCongHeaderDb { get; set; }
        public DbSet<ChamCongLine1> ChamCongLine1 { get; set; }
        public DbSet<HistoryResponse> HistoryResponses { get; set; }
        public DbSet<Login> Login { get; set; }
        public DbSet<ChamCongLineDb> ChamCongLines { get; set; }
        public DbSet<UserInfoDb> UserInfo { get; set; }
        public DbSet<UserGrpDb> UserGrp { get; set; }
        public DbSet<DeptsDb> Depts { get; set; }
        public DbSet<PersonnelsDb> Personnels { get; set; }
        public DbSet<ChamCongTypeDb> ChamCongTypes { get; set; }
        public DbSet<DonCongTacDb> DonCongTacs { get; set; }
        public DbSet<PersonnelsTypeDb> PersonnelsTypes { get; set; }
        public DbSet<LichNghiDb> LichNghiDbs { get; set; }
        public DbSet<NotificationDb> NotificationDbs { get; set; }
        public DbSet<NghiLeDb> NghiLeDbs { get; set; }
        public DbSet<HospitalDb>HospitalDbs { get; set; }
        public DbSet<FaceId> FaceIds { get; set; }
        public DbSet<LichLamViecDb>LichLamViecDb { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Các entity không có khóa chính
            modelBuilder.Entity<HistoryResponse>().HasNoKey();
            modelBuilder.Entity<PersonnelsTypeDb>().HasNoKey();
            modelBuilder.Entity<ChamCongHeaderDb>().HasNoKey();
            modelBuilder.Entity<Login>().HasNoKey();
            modelBuilder.Entity<ChamCongLineDb>().HasNoKey();
            modelBuilder.Entity<UserInfoDb>().HasNoKey();
            modelBuilder.Entity<UserGrpDb>().HasNoKey();
            modelBuilder.Entity<DeptsDb>().HasNoKey();
            modelBuilder.Entity<HospitalDb>().HasNoKey();
            // Cấu hình bảng và khóa chính của các entity
            modelBuilder.Entity<ChamCongLine1>().HasKey(c => new { c.MngChamCongPrkID, c.Buoi });
            modelBuilder.Entity<ChamCongLine1>(builder =>
            {
                builder.ToTable("MngChamCongLine3", "dbo");
            });

            modelBuilder.Entity<DonCongTacDb>(builder =>
            {
                builder.HasKey(e => e.MngDonCongTacPrkID);
                builder.HasOne(d => d.Personnels)
                    .WithMany()
                    .HasForeignKey(d => d.PsnPrkID);
                builder.ToTable("MngDonCongTac", "dbo"); // Đảm bảo rằng bảng có schema dbo
            });
            modelBuilder.Entity<LichNghiDb>(builder =>
            {
                builder.HasKey(e => e.LichNghiID);
                builder.HasOne(l => l.Personnels)
                    .WithMany()
                    .HasForeignKey(l => l.PsnPrkID)
                    .HasPrincipalKey(p => p.PsnPrkID);
                builder.ToTable("MngLichNghi", "dbo");
            });
            modelBuilder.Entity<ChamCongLineDb>(builder =>
            {
                builder.HasKey(e => e.MngChamCongPrkID);
                builder.ToTable("MngChamCongLine", "dbo"); // Đảm bảo rằng bảng có schema dbo
            });
            modelBuilder.Entity<NotificationDb>(builder =>
            {
                builder.HasKey(e => e.NotificationID);
                builder.ToTable("Notifications", "dbo");
                
            });
            modelBuilder.Entity<NghiLeDb>(builder =>
            {
                builder.HasKey(e => e.HolidayId);
                builder.ToTable("MngHolidays", "dbo");

            });
            modelBuilder.Entity<LichLamViecDb>().ToTable("MngLichLamViec", "dbo");
            modelBuilder.Entity<FaceId>().ToTable("MngFaceId", "dbo");  
            modelBuilder.Entity<HospitalDb>().ToTable("MngHospitals", "dbo");
            modelBuilder.Entity<PersonnelsDb>().HasKey(e => e.PsnPrkID);
            modelBuilder.Entity<PersonnelsDb>().ToTable("Dm_Personnels", "dbo");
            modelBuilder.Entity<DeptsDb>().ToTable("Dm_Depts", "dbo");
            modelBuilder.Entity<PersonnelsTypeDb>().ToTable("Dm_PersonTypes", "dbo");
            modelBuilder.Entity<ChamCongTypeDb>().ToTable("Enum_ChamCongType", "dbo");
            modelBuilder.Entity<ChamCongHeader1>().ToTable("MngChamCongHeader3", "dbo");
            modelBuilder.Entity<ChamCongHeaderDb>().ToTable("MngChamCongHeader", "dbo");
            modelBuilder.Entity<ChamCongHeaderDb>().ToTable("MngChamCongHeader", "dbo");
            // Cấu hình mối quan hệ giữa ChamCongLine1 và ChamCongHeader11
            modelBuilder.Entity<ChamCongLine1>()
                .HasOne(l => l.ChamCongHeader)
                .WithMany(h => h.ChamCongLines)
                .HasForeignKey(l => l.MngChamCongPrkID);

            modelBuilder.Entity<ChamCongHeader1>()
                .HasMany(h => h.ChamCongLines)
                .WithOne(l => l.ChamCongHeader)
                .HasForeignKey(l => l.MngChamCongPrkID);
        }
    }
}
