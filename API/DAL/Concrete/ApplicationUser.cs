using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Concrete
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool ChangePassword { get; set; }
    }
}
