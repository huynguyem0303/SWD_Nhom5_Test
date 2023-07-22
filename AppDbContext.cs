
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TableReservation.Models;

namespace TableReservation
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Menus> Menus { get; set; }
        public DbSet<Reservations> Reservations { get; set; }
        public DbSet<Restaurants> Restaurants { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Tables> Tables { get; set; }
        public DbSet<Users> Users { get; set; }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.EnableSensitiveDataLogging();
        //}
    }
}
