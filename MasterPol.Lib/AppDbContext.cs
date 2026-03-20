using Microsoft.EntityFrameworkCore;

namespace MasterPol.Lib
{
    public class AppDbContext : DbContext
    {
        public DbSet<PartnerCategory> PartnerCategories { get; set; }
        public DbSet<BusinessPartner> BusinessPartners { get; set; }
        public DbSet<MerchandiseItem> MerchandiseItems { get; set; }
        public DbSet<SaleRecord> SaleRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=zolotova_database;Username=app;Password=123456789");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка таблицы PartnerCategory
            modelBuilder.Entity<PartnerCategory>(entity =>
            {
                entity.ToTable("partner_types", "app");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("created_date");
            });

            // Настройка таблицы BusinessPartner
            modelBuilder.Entity<BusinessPartner>(entity =>
            {
                entity.ToTable("partners", "app");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PartnerTypeId).HasColumnName("type_id");
                entity.Property(e => e.CompanyName).HasColumnName("company_name").IsRequired();
                entity.Property(e => e.LegalAddress).HasColumnName("legal_address");
                entity.Property(e => e.Inn).HasColumnName("inn_number");
                entity.Property(e => e.DirectorName).HasColumnName("director_fullname");
                entity.Property(e => e.Phone).HasColumnName("contact_phone");
                entity.Property(e => e.Email).HasColumnName("contact_email");
                entity.Property(e => e.Rating).HasColumnName("partner_rating");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(e => e.Inn).IsUnique();

                entity.HasOne(e => e.PartnerType)
                    .WithMany(p => p.Partners)
                    .HasForeignKey(e => e.PartnerTypeId);
            });

            // Настройка таблицы MerchandiseItem
            modelBuilder.Entity<MerchandiseItem>(entity =>
            {
                entity.ToTable("products", "app");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Article).HasColumnName("product_article").IsRequired();
                entity.Property(e => e.Type).HasColumnName("product_type");
                entity.Property(e => e.Name).HasColumnName("product_name").IsRequired();
                entity.Property(e => e.MinPartnerPrice).HasColumnName("minimal_price");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.HasIndex(e => e.Article).IsUnique();
            });

            // Настройка таблицы SaleRecord
            modelBuilder.Entity<SaleRecord>(entity =>
            {
                entity.ToTable("sale_history", "app");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PartnerId).HasColumnName("partner_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity_sold");
                entity.Property(e => e.SaleDate).HasColumnName("sale_date");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.HasOne(e => e.Partner)
                    .WithMany(p => p.SalesHistory)
                    .HasForeignKey(e => e.PartnerId);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.SalesHistory)
                    .HasForeignKey(e => e.ProductId);
            });
        }
    }
}