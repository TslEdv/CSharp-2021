﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Domain.Config", b =>
                {
                    b.Property<int>("ConfigId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BoardSizeX")
                        .HasColumnType("int");

                    b.Property<int>("BoardSizeY")
                        .HasColumnType("int");

                    b.Property<string>("ConfigName")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ConfigStr")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreationTime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsRandom")
                        .HasColumnType("bit");

                    b.Property<int>("TouchRule")
                        .HasColumnType("int");

                    b.HasKey("ConfigId");

                    b.ToTable("Configs");
                });

            modelBuilder.Entity("Domain.ConfigShip", b =>
                {
                    b.Property<int>("ConfigShipId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ConfigId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("ShipId")
                        .HasColumnType("int");

                    b.HasKey("ConfigShipId");

                    b.HasIndex("ConfigId");

                    b.HasIndex("ShipId");

                    b.ToTable("ConfigShips");
                });

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.Property<int>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ConfigId")
                        .HasColumnType("int");

                    b.Property<string>("CreationTime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GameState")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ReplayId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("GameId");

                    b.HasIndex("ConfigId");

                    b.HasIndex("ReplayId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Domain.Replay", b =>
                {
                    b.Property<int>("ReplayId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Replays")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ReplayId");

                    b.ToTable("Replays");
                });

            modelBuilder.Entity("Domain.Ship", b =>
                {
                    b.Property<int>("ShipId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("ShipHeight")
                        .HasColumnType("int");

                    b.Property<int>("ShipLength")
                        .HasColumnType("int");

                    b.HasKey("ShipId");

                    b.ToTable("Ships");
                });

            modelBuilder.Entity("Domain.ConfigShip", b =>
                {
                    b.HasOne("Domain.Config", "Config")
                        .WithMany("ConfigShips")
                        .HasForeignKey("ConfigId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Ship", "Ship")
                        .WithMany("ConfigShips")
                        .HasForeignKey("ShipId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Config");

                    b.Navigation("Ship");
                });

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.HasOne("Domain.Config", "Config")
                        .WithMany()
                        .HasForeignKey("ConfigId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.Replay", "Replay")
                        .WithMany()
                        .HasForeignKey("ReplayId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Config");

                    b.Navigation("Replay");
                });

            modelBuilder.Entity("Domain.Config", b =>
                {
                    b.Navigation("ConfigShips");
                });

            modelBuilder.Entity("Domain.Ship", b =>
                {
                    b.Navigation("ConfigShips");
                });
#pragma warning restore 612, 618
        }
    }
}
