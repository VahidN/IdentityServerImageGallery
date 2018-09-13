using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DNT.IDP.DomainClasses
{
    public class User
    {
        [Key] 
        [MaxLength(50)] 
        public string SubjectId { get; set; }

        [MaxLength(100)] 
        [Required] 
        public string Username { get; set; }

        [MaxLength(100)] 
        public string Password { get; set; }

        [Required] 
        public bool IsActive { get; set; }

        public ICollection<UserClaim> UserClaims { get; set; } = new HashSet<UserClaim>();

        public ICollection<UserLogin> UserLogins { get; set; } = new HashSet<UserLogin>();
    }
}