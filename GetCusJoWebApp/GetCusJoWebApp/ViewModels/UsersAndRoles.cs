using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetCusJoWebApp.ViewModels
{
    public class UsersAndRoles
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Role> Roles { get; set; }
    }
}
