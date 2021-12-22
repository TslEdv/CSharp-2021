﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApp.DAL;

namespace WebApp.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20211222105945_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.13")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WebApp.Domain.ExtraTopping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("OrderPizzaId")
                        .HasColumnType("int");

                    b.Property<int>("ToppingId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderPizzaId");

                    b.HasIndex("ToppingId");

                    b.ToTable("ExtraToppings");
                });

            modelBuilder.Entity("WebApp.Domain.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClientName")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("CreationTime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("int");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("WebApp.Domain.OrderPizza", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("PizzaId")
                        .HasColumnType("int");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("PizzaId");

                    b.ToTable("OrderPizzas");
                });

            modelBuilder.Entity("WebApp.Domain.Pizza", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<bool>("Spicy")
                        .HasColumnType("bit");

                    b.Property<int?>("ToppingId")
                        .HasColumnType("int");

                    b.Property<bool>("Vegan")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("ToppingId");

                    b.ToTable("Pizzas");
                });

            modelBuilder.Entity("WebApp.Domain.PizzaToping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("PizzaId")
                        .HasColumnType("int");

                    b.Property<int>("ToppingId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PizzaId");

                    b.HasIndex("ToppingId");

                    b.ToTable("PizzaTopings");
                });

            modelBuilder.Entity("WebApp.Domain.Topping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.ToTable("Toppings");
                });

            modelBuilder.Entity("WebApp.Domain.ExtraTopping", b =>
                {
                    b.HasOne("WebApp.Domain.OrderPizza", "OrderPizza")
                        .WithMany("ExtraToppings")
                        .HasForeignKey("OrderPizzaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WebApp.Domain.Topping", "Topping")
                        .WithMany("ExtraToppings")
                        .HasForeignKey("ToppingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("OrderPizza");

                    b.Navigation("Topping");
                });

            modelBuilder.Entity("WebApp.Domain.OrderPizza", b =>
                {
                    b.HasOne("WebApp.Domain.Order", "Order")
                        .WithMany("OrderPizzas")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WebApp.Domain.Pizza", "Pizza")
                        .WithMany("OrderPizzas")
                        .HasForeignKey("PizzaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Pizza");
                });

            modelBuilder.Entity("WebApp.Domain.Pizza", b =>
                {
                    b.HasOne("WebApp.Domain.Topping", null)
                        .WithMany("Pizzas")
                        .HasForeignKey("ToppingId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("WebApp.Domain.PizzaToping", b =>
                {
                    b.HasOne("WebApp.Domain.Pizza", "Pizza")
                        .WithMany("PizzaTopings")
                        .HasForeignKey("PizzaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WebApp.Domain.Topping", "Topping")
                        .WithMany()
                        .HasForeignKey("ToppingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Pizza");

                    b.Navigation("Topping");
                });

            modelBuilder.Entity("WebApp.Domain.Order", b =>
                {
                    b.Navigation("OrderPizzas");
                });

            modelBuilder.Entity("WebApp.Domain.OrderPizza", b =>
                {
                    b.Navigation("ExtraToppings");
                });

            modelBuilder.Entity("WebApp.Domain.Pizza", b =>
                {
                    b.Navigation("OrderPizzas");

                    b.Navigation("PizzaTopings");
                });

            modelBuilder.Entity("WebApp.Domain.Topping", b =>
                {
                    b.Navigation("ExtraToppings");

                    b.Navigation("Pizzas");
                });
#pragma warning restore 612, 618
        }
    }
}
