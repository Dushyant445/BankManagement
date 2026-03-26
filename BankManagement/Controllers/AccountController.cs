using BankManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BankManagement.Controllers
{
    public class AccountController : Controller
    {


        SqlConnection conn = new SqlConnection("Data Source=DUSHYANT\\SQLEXPRESS; Initial Catalog=BankDb;Integrated Security=True");
	
        public ActionResult RegisterPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterPage(Customer C)
        {
            
            if (ModelState.IsValid)
            {
                //This is a pass encrypted method
                string encryptedPwd = EncryptPassword(C.password);

   
                conn.Open();
                SqlCommand cmd = new SqlCommand("data_customer", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", C.name);
                cmd.Parameters.AddWithValue("@email", C.email);
                cmd.Parameters.AddWithValue("@mobile", C.mobile);
                cmd.Parameters.AddWithValue("@password", encryptedPwd);
                cmd.ExecuteNonQuery();
                conn.Close();

                return RedirectToAction("Login");
            }
            return View(C);
        }

        
        private string EncryptPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            // If pass hash campare with encrypted pass then only login successfull otherwise not 
            string encryptedPwd = EncryptPassword(password);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;  
            cmd.CommandText =  "SELECT customerid, name FROM tblcustomers WHERE email=@email AND password=@password";

            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", encryptedPwd);

            conn.Open();
            SqlDataReader r = cmd.ExecuteReader();

            if (r.Read())
            {
                Session["CustomerId"] = r["customerid"]; // Session is create automatically in ASP.NET MVC when we store data inside the session obj  after succesfull login//Seesin Is Created 
                Session["CustomerName"] = r["name"].ToString();
                conn.Close();
                return RedirectToAction("Dashboard");
            }

            conn.Close();
            ViewBag.Error = "Invalid Email or Password";
            return View();
        }

        public ActionResult Dashboard()
        {
            if (Session["CustomerId"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        public ActionResult Profileview()
        {
            if (Session["CustomerId"] == null)
            {
                return RedirectToAction("Login");
            }

            int Custoid = Convert.ToInt32(Session["CustomerId"]);

            SqlCommand cmd = new SqlCommand("select name, email,mobile from tblcustomers where customerid = @customerid", conn);
            cmd.Parameters.AddWithValue("@customerid", Custoid);
            conn.Open();
            SqlDataReader r = cmd.ExecuteReader();

            Customer c = new Customer();

            if (r.Read()) {

                c.name = r["name"].ToString();
                c.email = r["email"].ToString();
                c.mobile = r["mobile"].ToString();
            }
            conn.Close();
            return View(c);
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (Session["CustomerId"] == null)
            {
                return RedirectToAction("Login");
            }
            int Custoid = Convert.ToInt32(Session["CustomerId"]);
            conn.Open();
            SqlCommand cmd = new SqlCommand("data_edit", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@customerid", Custoid);
       
            SqlDataReader r = cmd.ExecuteReader();

            Customer c = new Customer();

            if (r.Read())
            {
                c.name = r["name"].ToString();
                c.email = r["email"].ToString();
                c.mobile = r["mobile"].ToString() ;
            
            }
            conn.Close();
            return View(c);
        }
        [HttpPost]
        public ActionResult Update(Customer c)
        {
            int cid = Convert.ToInt32(Session["CustomerId"]);

            conn.Open();
            SqlCommand cmd = new SqlCommand("data_update", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@customerid", cid);
            cmd.Parameters.AddWithValue("@name", c.name);
            cmd.Parameters.AddWithValue("@email", c.email);
            cmd.Parameters.AddWithValue("@mobile", c.mobile); 
            cmd.ExecuteNonQuery();
            conn.Close();

            return RedirectToAction("Profileview");
        }
        public ActionResult Cancel()
        {
          return RedirectToAction("Profileview");
        }

        [HttpGet]
        public ActionResult Account()
        {
            if (Session["CustomerId"] == null) 
            {  
                return RedirectToAction("Login");
            }

            return View();
        }
        [HttpPost]
        public ActionResult AccountCreate(account A)
        {
            if (Session["CustomerId"] == null)
            {
                return RedirectToAction("Login");
            }

            int Custoid = Convert.ToInt32(Session["CustomerId"]);
            long accNo = DateTime.Now.Ticks;

            SqlCommand cmd = new SqlCommand("create_accounts", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@customerid", Custoid);
            cmd.Parameters.AddWithValue("@accountnumber", accNo);
            cmd.Parameters.AddWithValue("@accounttype", A.accounttype);
            cmd.Parameters.AddWithValue("@balance", A.balance);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            return RedirectToAction("Dashboard");
        }

    }


}
