using System;
using System.Collections.Generic;
using System.Text;

using InvoiceGenerator.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace InvoiceGenerator.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
        public DbSet<tblInvoice> Invoices { get; set; }
        public DbSet<tblInvoiceItems> InvoiceItems { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
                
        }

        public ApplicationDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        {
            base.OnConfiguring(optionsBuilder);
           
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("Inv");
            // configure Tables
            builder.Entity<tblInvoice>(ConfigureInvoice);
            builder.Entity<tblLookup>(ConfigureLookup);
        }

        private void ConfigureLookup(EntityTypeBuilder<tblLookup> obj)
        {
            obj.ToTable("Lookup");
            obj.Property(p => p.Id).IsRequired();
            obj.Property(p => p.RowVersion).IsRowVersion();
            obj.Property(p => p.Note).HasMaxLength(250);
            obj.Property(p => p.Lookup).HasMaxLength(25);
            obj.Property(p => p.Value).HasMaxLength(25);
        }

        private void ConfigureInvoice(EntityTypeBuilder<tblInvoice> obj)
        {
            obj.ToTable("Invoice");
            obj.Property(p => p.Id).IsRequired();
            obj.Property(p => p.RowVersion).IsRowVersion();
            obj.Property(p => p.Code).HasMaxLength(125);
            obj.HasIndex(p => p.Code).IsUnique().HasName("Idx_InvoiceCode");
            obj.Property(p => p.Date).HasMaxLength(25);
            obj.Property(p => p.From).HasMaxLength(255);
            obj.Property(p => p.Price).HasMaxLength(25);
            obj.Property(p => p.Status).HasMaxLength(25);
            obj.Property(p => p.Tax).HasMaxLength(25);
            obj.Property(p => p.To).HasMaxLength(255);
            obj.Property(p => p.ToEmail).HasMaxLength(100);
            obj.Property(p => p.Total).HasMaxLength(25);
        }
    }
}
