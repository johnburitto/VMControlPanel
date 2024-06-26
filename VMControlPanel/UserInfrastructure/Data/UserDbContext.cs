﻿using Core.Entities;
using Microsoft.EntityFrameworkCore;
using UserInfrastructure.Configurations;

namespace UserInfrastructure.Data
{
    public class UserDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

        public UserDbContext()
        {

        }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) 
        { 
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
