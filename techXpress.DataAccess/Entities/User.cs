using System;
using System.Collections.Generic;

namespace techXpress.DataAccess.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Fname { get; set; }
        public string? Lname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        // Navigation properties
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}