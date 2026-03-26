using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BankManagement.Models
{
    public class Customer
    {
        public int customerid { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Please Enter Full Name")]
        [StringLength(50, MinimumLength = 2)]
        public string name { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Please Enter Email")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address")]
        public string email { get; set; }

        [DisplayName("Mobile Number")]
        [Required(ErrorMessage = "Please Enter Mobile Number")]
        public string mobile { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Please Enter Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [NotMapped]
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Please Enter Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Password and Confirm Password do not match")]
        public string confirmpassword { get; set; }
    }
}