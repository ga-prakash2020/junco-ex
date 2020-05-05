using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GetCusJoWebApp.ViewModels
{
    public class UserRoles
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string RoleIds { get; set; }
    }
}
