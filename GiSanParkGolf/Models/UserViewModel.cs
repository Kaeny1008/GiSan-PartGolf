using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiSanParkGolf.Models
{
    public class UserViewModel
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}