﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Routine.Api.Data;

namespace Routine.Api.Migrations
{
    [DbContext(typeof(RoutineDbContext))]
    partial class RoutineDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3");

            modelBuilder.Entity("Routine.Api.Entities.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Introduction")
                        .HasColumnType("TEXT")
                        .HasMaxLength(500);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Companies");

                    b.HasData(
                        new
                        {
                            Id = new Guid("48a7a966-4c78-4ce5-a300-435507b6cadc"),
                            Introduction = "Don not be evil",
                            Name = "Google"
                        },
                        new
                        {
                            Id = new Guid("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"),
                            Introduction = "Create Company",
                            Name = "Microsoft"
                        },
                        new
                        {
                            Id = new Guid("34dc1695-7e1a-4687-824b-5af45aa92342"),
                            Introduction = "no",
                            Name = "Apple"
                        });
                });

            modelBuilder.Entity("Routine.Api.Entities.Employee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("TEXT");

                    b.Property<string>("EmployeeNo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<int>("Gender")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Employees");

                    b.HasData(
                        new
                        {
                            Id = new Guid("d8a53929-be70-4f51-8fb8-442c3b2c2d6a"),
                            CompanyId = new Guid("48a7a966-4c78-4ce5-a300-435507b6cadc"),
                            DateOfBirth = new DateTime(2001, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeNo = "SF112",
                            FirstName = "Nike",
                            Gender = 1,
                            LastName = "Garter"
                        },
                        new
                        {
                            Id = new Guid("656c985a-bcda-4d86-917c-e3775d6ed94c"),
                            CompanyId = new Guid("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"),
                            DateOfBirth = new DateTime(2002, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeNo = "01487",
                            FirstName = "Jonty",
                            Gender = 1,
                            LastName = "Wang"
                        },
                        new
                        {
                            Id = new Guid("ffc7b7ca-52f1-4bb6-a935-f93d238b9733"),
                            CompanyId = new Guid("34dc1695-7e1a-4687-824b-5af45aa92342"),
                            DateOfBirth = new DateTime(2001, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeNo = "11929",
                            FirstName = "mark",
                            Gender = 1,
                            LastName = "Wang"
                        },
                        new
                        {
                            Id = new Guid("a640c260-3ad6-4aba-b86e-817353101fd5"),
                            CompanyId = new Guid("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"),
                            DateOfBirth = new DateTime(1987, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeNo = "12341",
                            FirstName = "alice",
                            Gender = 2,
                            LastName = "Wang"
                        },
                        new
                        {
                            Id = new Guid("cf58b318-4c49-4f7a-bfaa-5227a1e48afd"),
                            CompanyId = new Guid("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"),
                            DateOfBirth = new DateTime(2001, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeNo = "11324",
                            FirstName = "alice",
                            Gender = 1,
                            LastName = "jonty"
                        },
                        new
                        {
                            Id = new Guid("8b320533-67e5-4db6-bf3d-d87a24311d25"),
                            CompanyId = new Guid("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"),
                            DateOfBirth = new DateTime(2000, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeNo = "34521",
                            FirstName = "alice",
                            Gender = 2,
                            LastName = "wang"
                        });
                });

            modelBuilder.Entity("Routine.Api.Entities.Employee", b =>
                {
                    b.HasOne("Routine.Api.Entities.Company", "Company")
                        .WithMany("Employees")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
