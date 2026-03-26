using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankManagement.Models
{
    public class Transaction
    {
        public int transactionid { get; set; }

        public int accountid { get; set; }

        [Required]
        public string transactiontype { get; set; }

        [Required]
        
        public decimal amount { get; set; }

        public string remarks { get; set; }
    }
}