using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using eVoucher_Entities.EntityModels;

namespace eVoucher_Entities.EntityModels
{
    public partial class eVoucherContext : DbContext
    {
        public eVoucherContext()
        {
        }

        public eVoucherContext(DbContextOptions<eVoucherContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblEvoucher> TblEvouchers { get; set; } = null!;
        public virtual DbSet<TblaccessToken> TblaccessTokens { get; set; } = null!;
        public virtual DbSet<TblpaymentMethod> TblpaymentMethods { get; set; } = null!;
        public virtual DbSet<TblpromoCode> TblpromoCodes { get; set; } = null!;
        public virtual DbSet<Tblpurchase> Tblpurchases { get; set; } = null!;
        public virtual DbSet<Tbluser> Tblusers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<TblEvoucher>(entity =>
            {
                entity.HasKey(e => e.VoucherNo)
                    .HasName("PRIMARY");

                entity.ToTable("tbl_evoucher");

                entity.Property(e => e.VoucherNo)
                    .HasMaxLength(50)
                    .HasColumnName("voucher_No");

                entity.Property(e => e.Amount)
                    .HasPrecision(10, 2)
                    .HasColumnName("amount");

                entity.Property(e => e.BuyType)
                    .HasMaxLength(50)
                    .HasColumnName("buy_type");

                entity.Property(e => e.Description)
                    .HasMaxLength(512)
                    .HasColumnName("description");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("expiry_date");

                entity.Property(e => e.GiftPerUserLimit).HasColumnName("gift_per_user_limit");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImagePath)
                    .HasMaxLength(256)
                    .HasColumnName("image_path");

                entity.Property(e => e.MaxLimit).HasColumnName("max_Limit");

                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50)
                    .HasColumnName("payment_method")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.Price)
                    .HasPrecision(10, 2)
                    .HasColumnName("price");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Title)
                    .HasMaxLength(256)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<TblaccessToken>(entity =>
            {
                entity.ToTable("tblaccess_token");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccessToken)
                    .HasMaxLength(1024)
                    .HasColumnName("access_token")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.AccessTokenExpiry)
                    .HasColumnType("datetime")
                    .HasColumnName("access_token_expiry");

                entity.Property(e => e.RefreshToken)
                    .HasMaxLength(512)
                    .HasColumnName("refresh_token")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.RefreshTokenExpiry)
                    .HasColumnType("datetime")
                    .HasColumnName("refresh_token_expiry");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<TblpaymentMethod>(entity =>
            {
                entity.ToTable("tblpayment_method");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DiscountPercentage).HasColumnName("discount_percentage");

                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50)
                    .HasColumnName("payment_method")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("'1'");
            });

            modelBuilder.Entity<TblpromoCode>(entity =>
            {
                entity.HasKey(e => e.PromoCode)
                    .HasName("PRIMARY");

                entity.ToTable("tblpromo_code");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("expiry_date");

                entity.Property(e => e.OwnerName)
                    .HasMaxLength(300)
                    .HasColumnName("owner_name")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.OwnerPhone)
                    .HasMaxLength(300)
                    .HasColumnName("owner_phone")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.PromoCode)
                    .HasMaxLength(50)
                    .HasColumnName("promo_code");

                entity.Property(e => e.QrImage)
                    .HasMaxLength(512)
                    .HasColumnName("qr_image")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.VoucherAmount)
                    .HasColumnType("double(10,2)")
                    .HasColumnName("voucher_amount");

                entity.Property(e => e.VoucherNo)
                    .HasMaxLength(50)
                    .HasColumnName("voucher_No");

                entity.Property(e=>e.purchase_id)
                .HasMaxLength(45)
                .HasColumnName("purchase_id");
            });

            modelBuilder.Entity<Tblpurchase>(entity =>
            {
                entity.HasKey(e => e.PurchaseId)
                    .HasName("PRIMARY");

                entity.ToTable("tblpurchases");

                entity.Property(e => e.PurchaseId)
                    .HasMaxLength(20)
                    .HasColumnName("purchase_id");

                entity.Property(e => e.Amount)
                    .HasPrecision(10, 2)
                    .HasColumnName("amount");

                entity.Property(e => e.BuyType)
                    .HasMaxLength(20)
                    .HasColumnName("buy_type")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.BuyerName)
                    .HasMaxLength(300)
                    .HasColumnName("buyer_name")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.BuyerPhone)
                    .HasMaxLength(45)
                    .HasColumnName("buyer_phone");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("expiry_date");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(45)
                    .HasColumnName("payment_method");

                entity.Property(e => e.Price)
                    .HasPrecision(10, 2)
                    .HasColumnName("price");

                entity.Property(e => e.PurchaseDate)
                    .HasColumnType("datetime")
                    .HasColumnName("purchase_date");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Total)
                    .HasPrecision(10, 2)
                    .HasColumnName("total");

                entity.Property(e => e.VoucherNo)
                    .HasMaxLength(50)
                    .HasColumnName("voucher_no");
            });

            modelBuilder.Entity<Tbluser>(entity =>
            {
                entity.ToTable("tblusers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Displayname)
                    .HasMaxLength(256)
                    .HasColumnName("displayname")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.Password)
                    .HasMaxLength(512)
                    .HasColumnName("password")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Usename)
                    .HasMaxLength(50)
                    .HasColumnName("usename")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
