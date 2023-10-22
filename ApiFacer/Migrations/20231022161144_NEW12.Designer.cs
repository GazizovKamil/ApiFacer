﻿// <auto-generated />
using System;
using ApiFacer.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ApiFacer.Migrations
{
    [DbContext(typeof(ApiDB))]
    [Migration("20231022161144_NEW12")]
    partial class NEW12
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ApiFacer.Classes.Events", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentEventId")
                        .HasColumnType("int");

                    b.Property<int>("authorId")
                        .HasColumnType("int");

                    b.Property<string>("path")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ParentEventId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("ApiFacer.Classes.Images", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("authorId")
                        .HasColumnType("int");

                    b.Property<int>("eventId")
                        .HasColumnType("int");

                    b.Property<string>("path")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("eventId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("ApiFacer.Classes.Logins", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("id_role")
                        .HasColumnType("int");

                    b.Property<string>("ipAdress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("sessionkey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("userId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Logins");
                });

            modelBuilder.Entity("ApiFacer.Classes.Roles", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            name = "Admin"
                        },
                        new
                        {
                            Id = 2,
                            name = "Фотограф"
                        },
                        new
                        {
                            Id = 3,
                            name = "Гость"
                        });
                });

            modelBuilder.Entity("ApiFacer.Classes.UserImages", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ImageId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("UserImages");
                });

            modelBuilder.Entity("ApiFacer.Classes.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("faceDescriptor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("first_name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("id_role")
                        .HasColumnType("int");

                    b.Property<string>("last_name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("login")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("surname")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            first_name = "Арсений",
                            id_role = 1,
                            last_name = "А",
                            login = "admin",
                            password = "admin",
                            surname = "Артюхин"
                        });
                });

            modelBuilder.Entity("ApiFacer.Classes.Events", b =>
                {
                    b.HasOne("ApiFacer.Classes.Events", "ParentEvent")
                        .WithMany()
                        .HasForeignKey("ParentEventId");

                    b.Navigation("ParentEvent");
                });

            modelBuilder.Entity("ApiFacer.Classes.Images", b =>
                {
                    b.HasOne("ApiFacer.Classes.Events", "EventId")
                        .WithMany("Images")
                        .HasForeignKey("eventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EventId");
                });

            modelBuilder.Entity("ApiFacer.Classes.Events", b =>
                {
                    b.Navigation("Images");
                });
#pragma warning restore 612, 618
        }
    }
}
