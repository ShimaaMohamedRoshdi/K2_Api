using AccountDocApi.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace AccountDocApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Document> Documents => Set<Document>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Account Entity
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.AccountNo);
                entity.Property(e => e.AccountNo).HasMaxLength(50);
                entity.Property(e => e.CustomerId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.AccountType).HasMaxLength(50);
                entity.Property(e => e.AccountStatus).HasMaxLength(50);
                entity.Property(e => e.Branch).HasMaxLength(100);
            });

            // Configure Document Entity
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.DocumentId);
                entity.Property(e => e.DocumentId).ValueGeneratedOnAdd();
                entity.Property(e => e.AccountNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DocumentType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DocumentName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.DocumentUrl).HasMaxLength(500);

                entity.HasOne(d => d.Account)
                      .WithMany(a => a.Documents)
                      .HasForeignKey(d => d.AccountNo)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Sample binary signature file content (Base64 decoded bytes)
            byte[] sampleSignatureBytes1 = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");
            byte[] sampleSignatureBytes2 = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==");

            // Seed Accounts
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    AccountNo = "ACC1001",
                    CustomerId = "CUST-8832",
                    CustomerName = "Ahmed Hassan",
                    AccountType = "Savings",
                    AccountStatus = "Active",
                    Branch = "Cairo Main Branch"
                },
                new Account
                {
                    AccountNo = "ACC1002",
                    CustomerId = "CUST-9941",
                    CustomerName = "Sara Mahmoud",
                    AccountType = "Checking",
                    AccountStatus = "Active",
                    Branch = "Alexandria Branch"
                },
                new Account
                {
                    AccountNo = "ACC1003",
                    CustomerId = "CUST-7715",
                    CustomerName = "Mohamed Ali",
                    AccountType = "Corporate",
                    AccountStatus = "Pending",
                    Branch = "Giza Branch"
                }
            );

            // Seed Documents
            modelBuilder.Entity<Document>().HasData(
                new Document
                {
                    DocumentId = 1,
                    AccountNo = "ACC1001",
                    DocumentType = "Signature",
                    DocumentName = "Signature_ACC1001.png",
                    DocumentUrl = "http://localhost:5000/documents/signatures/ACC1001.png",
                    CreatedDate = new DateTime(2026, 9, 21, 10, 0, 0, DateTimeKind.Utc),
                    FileContent = sampleSignatureBytes1
                },
                new Document
                {
                    DocumentId = 2,
                    AccountNo = "ACC1002",
                    DocumentType = "Signature",
                    DocumentName = "Signature_ACC1002.png",
                    DocumentUrl = "http://localhost:5000/documents/signatures/ACC1002.png",
                    CreatedDate = new DateTime(2026, 9, 21, 11, 30, 0, DateTimeKind.Utc),
                    FileContent = sampleSignatureBytes2
                },
                new Document
                {
                    DocumentId = 3,
                    AccountNo = "ACC1003",
                    DocumentType = "Signature",
                    DocumentName = "Signature_ACC1003.png",
                    DocumentUrl = "http://localhost:5000/documents/signatures/ACC1003.png",
                    CreatedDate = new DateTime(2026, 9, 21, 12, 15, 0, DateTimeKind.Utc),
                    FileContent = sampleSignatureBytes1
                }
            );
        }
    }
}
