using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using vkpolls.auth.Domain.Enum;

namespace vkpolls.auth.Domain
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public required string  UserIdentityId { get; set; }
        public string? ProfilePicUrl { get; set; }
        public string? Name { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Gender Gender { get; set; }
        public DateOnly DOB { get; set; }
        public bool CanCreatePoll { get; set; } = true; //user can create a poll or not, not sure
        public bool IsActive { get; set; } = true; // user delete
        public int NoOfPollsCreated { get; set; }
    }
}
