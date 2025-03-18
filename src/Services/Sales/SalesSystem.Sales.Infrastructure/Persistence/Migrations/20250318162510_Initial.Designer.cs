﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SalesSystem.Sales.Infrastructure.Persistence;

#nullable disable

namespace SalesSystem.Sales.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(SalesDbContext))]
    [Migration("20250318162510_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("sales")
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SalesSystem.Sales.Domain.Entities.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(160)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Discount")
                        .HasColumnType("MONEY");

                    b.Property<decimal>("Price")
                        .HasColumnType("MONEY");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<Guid?>("VoucherId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("VoucherIsUsed")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("VoucherId");

                    b.ToTable("Orders", "sales");
                });

            modelBuilder.Entity("SalesSystem.Sales.Domain.Entities.OrderItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(160)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("MONEY");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems", "sales");
                });

            modelBuilder.Entity("SalesSystem.Sales.Domain.Entities.Voucher", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(160)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<decimal?>("Percentual")
                        .HasColumnType("MONEY");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<bool>("Used")
                        .HasColumnType("bit");

                    b.Property<decimal?>("Value")
                        .HasColumnType("MONEY");

                    b.HasKey("Id");

                    b.ToTable("Vouchers", "sales");
                });

            modelBuilder.Entity("SalesSystem.Sales.Domain.Entities.Order", b =>
                {
                    b.HasOne("SalesSystem.Sales.Domain.Entities.Voucher", "Voucher")
                        .WithMany("Orders")
                        .HasForeignKey("VoucherId");

                    b.Navigation("Voucher");
                });

            modelBuilder.Entity("SalesSystem.Sales.Domain.Entities.OrderItem", b =>
                {
                    b.HasOne("SalesSystem.Sales.Domain.Entities.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("SalesSystem.Sales.Domain.Entities.Order", b =>
                {
                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("SalesSystem.Sales.Domain.Entities.Voucher", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
