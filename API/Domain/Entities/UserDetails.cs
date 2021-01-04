using System.Collections.Generic;

namespace Domain.Entities
{
    public partial class UserDetails : BaseEntity
    {
        public UserDetails()
        {
        }
       
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
