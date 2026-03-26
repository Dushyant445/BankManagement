using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankManagement.Models
{
    public class account
    {
        public int accountid { get; set; }

        public int customerid { get; set; }

        public string accountnumber { get; set; }

        [Required]
        public string accounttype { get; set; }


        [Required]
        [Range(500, Double.MaxValue, ErrorMessage = "Minimum Balance 500 Required")]
        public decimal balance { get; set; }
    }
}