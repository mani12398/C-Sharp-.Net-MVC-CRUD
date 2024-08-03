using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_CRUD.Models
{
    public class LoginSignUp
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Mobile_No { get; set; }
        public string  UserPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public bool RememberMe { get; set; }

    }
}