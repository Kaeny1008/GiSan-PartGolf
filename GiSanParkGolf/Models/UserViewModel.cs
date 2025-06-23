using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiSanParkGolf.Models
{
    public class UserViewModel
    {
        public string UserID { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string UserWClass { get; set; }
        public string SelectID { get; set; }
    }

    public class SelectUserViewModel
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public int UserNumber { get; set; }
        public int UserGender { get; set; }
        public string UserAddress { get; set; }
        public string UserAddress2 { get; set; }
        public DateTime UserRegistrationDate { get; set; }
        public string UserNote { get; set; }
        public string UserWClass { get; set; }
    }
}