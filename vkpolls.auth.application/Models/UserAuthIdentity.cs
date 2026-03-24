using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vkpolls.auth.Application.Models
{
    public class UserAuthIdentity
    {
        public required string identifier { get; set; }
        [MinLength(6)]
        public required string password { get; set; }
    }
}
