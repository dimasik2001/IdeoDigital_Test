using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdeoDigital_TestProject.Models
{
    public class MemberViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public object Phone { get; set; }
        public string IconUrl{ get; set; }
    }
}