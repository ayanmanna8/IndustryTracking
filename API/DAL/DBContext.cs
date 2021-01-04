using DAL.Concrete;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
namespace DAL
{
    public class DBContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IUserService _userService;
        public DBContext(DbContextOptions<DBContext> options, IUserService userService)
            : base(options)

        {

            if (userService != null)
                _userService = userService;
        }
        public DbSet<UserDetails> UserDetails { get; set; }
    }

    public interface IUserService
    {
        string GetUserName();
    }
}
