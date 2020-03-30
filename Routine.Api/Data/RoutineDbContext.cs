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
                .IsRequired()
                .HasMaxLength(10);
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

            //modelBuilder.Entity<Company>()
            //    .HasData(new Company
            //    {
            //        Id=Guid.Parse("bbdee09c-089b-4")
            //    });
        }
    }
}
