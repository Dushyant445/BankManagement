using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankManagement.Controllers
{
       public class TransactionController : Controller
        {

        SqlConnection conn = new SqlConnection("Data Source=DUSHYANT\\SQLEXPRESS; Initial Catalog=BankDb;Integrated Security=True");

        [HttpGet]
        public ActionResult Deposit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Deposit(int accountid, decimal amount)
        {
            if (amount <= 0)
            {
                ViewBag.Msg = "Invalid amount";
                return View();
            }

            conn.Open();

            // Update balance
            SqlCommand cmd1 = new SqlCommand("UPDATE tblaccounts SET balance = balance + @amount WHERE accountid = @accountid", conn);
            cmd1.Parameters.AddWithValue("@amount", amount);
            cmd1.Parameters.AddWithValue("@accountid", accountid);
            cmd1.ExecuteNonQuery();

            // Insert transaction
            SqlCommand cmd2 = new SqlCommand("INSERT INTO tbltransactions(accountid, transactiontype, amount) VALUES(@accountid, 'Deposit', @amount)", conn);
            cmd2.Parameters.AddWithValue("@accountid", accountid);
            cmd2.Parameters.AddWithValue("@amount", amount);
            cmd2.ExecuteNonQuery();

            conn.Close();

            ViewBag.Msg = "Deposit Successful";
            return View();
        }
        [HttpGet]
        public ActionResult Withdraw()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Withdraw(int accountid, decimal amount)
        {
            conn.Open();

            SqlCommand chk = new SqlCommand(
                "SELECT balance FROM tblaccounts WHERE accountid=@aid", conn);
            chk.Parameters.AddWithValue("@aid", accountid);

            decimal balance = Convert.ToDecimal(chk.ExecuteScalar());

            if (amount <= 0 || amount > balance)
            {
                ViewBag.Msg = "Insufficient Balance";
                conn.Close();
                return View();
            }

            // Deduct balance
            SqlCommand cmd1 = new SqlCommand(
                "UPDATE tblaccounts SET balance = balance - @amt WHERE accountid=@aid", conn);
            cmd1.Parameters.AddWithValue("@amt", amount);
            cmd1.Parameters.AddWithValue("@aid", accountid);
            cmd1.ExecuteNonQuery();

            // Transaction log
            SqlCommand cmd2 = new SqlCommand(
                "INSERT INTO tbltransactions(accountid, transactiontype, amount) VALUES(@aid, 'Withdraw', @amt)", conn);
            cmd2.Parameters.AddWithValue("@aid", accountid);
            cmd2.Parameters.AddWithValue("@amt", amount);
            cmd2.ExecuteNonQuery();

            conn.Close();

            ViewBag.Msg = "Withdraw Successful";
            return View();
        }
        public ActionResult HistoryAccount()
        {
            if (Session["CustomerId"] == null)
                return RedirectToAction("Login", "Account");

            int customerId = Convert.ToInt32(Session["CustomerId"]);

            SqlCommand cmd = new SqlCommand(@"
        SELECT t.transactiondate, t.transactiontype, t.amount, a.accountnumber
        FROM tbltransactions t
        INNER JOIN tblaccounts a ON t.accountid = a.accountid
        WHERE a.customerid = @cid", conn);

            cmd.Parameters.AddWithValue("@cid", customerId);

            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            List<dynamic> list = new List<dynamic>();
            while (dr.Read())
            {
                list.Add(new
                {
                    Name = dr["Name"],
                    Date = dr["transactiondate"],
                    Type = dr["transactiontype"],
                    Amount = dr["amount"],
                    Account = dr["accountnumber"]
                });
            }

            conn.Close();

            return View(list);
        }



    }
}