using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class AppUser
    {
        [Key]
        public string UID { get; set; }
        public string UserName { get; set; }
    }
}
