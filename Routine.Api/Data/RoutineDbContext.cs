using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Routine.Api.Entities;

namespace Routine.Api.Data
{
    public class RoutineDbContext:DbContext
    {
        public RoutineDbContext(DbContextOptions<RoutineDbContext>options)
        :base(options)
        {
            
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Company>()
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<Company>()
                .Property(x => x.Introduction)
                .HasMaxLength(500);

            modelBuilder.Entity<Employee>()
                .Property(x => x.EmployeeNo)
                .IsRequired();
            modelBuilder.Entity<Employee>()
                .Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<Employee>()
                .Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(50);

            //指明关系
            modelBuilder.Entity<Employee>()
                .HasOne(x => x.Company)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .HasData(new Company
                {
                    Id = Guid.Parse("48a7a966-4c78-4ce5-a300-435507b6cadc"),
                    Name ="Google",
                    Introduction = "Don not be evil",
                },new Company
                {
                    Id = Guid.Parse("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"),
                    Name = "Microsoft",
                    Introduction = "Create Company",
                }, new Company
                {
                    Id = Guid.Parse("34dc1695-7e1a-4687-824b-5af45aa92342"),
                    Name = "Apple",
                    Introduction = "no",
                });

            modelBuilder.Entity<Employee>()
                .HasData(new Employee
                {
                    Id=Guid.Parse("d8a53929-be70-4f51-8fb8-442c3b2c2d6a"),
                    CompanyId = Guid.Parse("48a7a966-4c78-4ce5-a300-435507b6cadc"),
                    DateOfBirth = new DateTime(2001, 12, 1),
                    EmployeeNo = "SF112",
                    FirstName = "Nike",
                    LastName = "Garter",
                    Gender = Gender.男
                },
                    new Employee
                    {
                        Id = Guid.Parse("656c985a-bcda-4d86-917c-e3775d6ed94c"),
                        CompanyId = Guid.Parse("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"),
                        DateOfBirth = new DateTime(2002, 3, 21),
                        EmployeeNo = "01487",
                        FirstName = "Jonty",
                        LastName = "Wang",
                        Gender = Gender.男
                    },
                    new Employee
                    {
                        Id = Guid.Parse("ffc7b7ca-52f1-4bb6-a935-f93d238b9733"),
                        CompanyId = Guid.Parse("34dc1695-7e1a-4687-824b-5af45aa92342"),
                        DateOfBirth = new DateTime(2001, 4, 21),
                        EmployeeNo = "11929",
                        FirstName = "mark",
                        LastName = "Wang",
                        Gender = Gender.男
                    },
                    new Employee
                    {
                        Id = Guid.Parse("a640c260-3ad6-4aba-b86e-817353101fd5"),
                        CompanyId = Guid.Parse("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"),
                        DateOfBirth = new DateTime(1987, 4, 21),
                        EmployeeNo = "12341",
                        FirstName = "alice",
                        LastName = "Wang",
                        Gender = Gender.女
                    }, new Employee
                    {
                        Id = Guid.Parse("cf58b318-4c49-4f7a-bfaa-5227a1e48afd"),
                        CompanyId = Guid.Parse("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"),
                        DateOfBirth = new DateTime(2001, 5, 21),
                        EmployeeNo = "11324",
                        FirstName = "alice",
                        LastName = "jonty",
                        Gender = Gender.男
                    }, new Employee
                    {
                        Id = Guid.Parse("8b320533-67e5-4db6-bf3d-d87a24311d25"),
                        CompanyId = Guid.Parse("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"),
                        DateOfBirth = new DateTime(2000, 7, 21),
                        EmployeeNo = "34521",
                        FirstName = "alice",
                        LastName = "wang",
                        Gender = Gender.女
                    });
        }
    }
}
