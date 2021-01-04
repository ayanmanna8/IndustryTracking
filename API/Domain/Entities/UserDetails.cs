using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entities
{
    public partial class UserDetails : BaseEntity
    {
        public UserDetails()
        {
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string EmailId { get; set; }
        public string AuthKey { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string FullName { get; set; }
        public string Organization { get; set; }
        public string Telephone { get; set; }
        public bool IsAdmin { get; set; }
    }
}
