using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL
{
    public partial class AppDBContext: DbContext
    {
        public AppDBContext(DbContextOptions options)
            : base(options)
        {
        }
        public DbSet<UserDetails> UserDetails { get; set; }
    }
}
