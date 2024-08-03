using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using MVC_CRUD.Models;

namespace MVC_CRUD.Controllers
{
    [Authorize]
    [CustomAuthorize]// Ensure all actions require authentication
    public class ProductController : BaseController
    {
        readonly string connectionString = ConnectionString.DatabaseString.ConnectionString;
        private void CheckSession()
        {
            if (Session["Id"] == null)
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                Response.Redirect(Url.Action("Login", "Login"));
            }
        }
        [HttpGet]
        public ActionResult Index()
        {
            CheckSession();
            
            // Check if user is authenticated and session variables are present
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * FROM Products", conn);
                sqlDataAdapter.Fill(dt);
            }

            // Retrieve user details from Session (assuming you stored them in Session in LoginController)
            ViewBag.Id=Session["ID"];
            ViewBag.Username = Session["Username"];
            ViewBag.Email = Session["Email"];
            ViewBag.Mobile_No = Session["Mobile_No"];
            ViewBag.UserRole = Session["UserRole"];

            return View(dt);

        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string insertQuery = "INSERT INTO products (Product_Name, Product_Price, Product_Count) VALUES (@Product_Name, @Product_Price, @Product_Count)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@Product_Name", product.Product_Name);
                    insertCmd.Parameters.AddWithValue("@Product_Price", product.Product_Price);
                    insertCmd.Parameters.AddWithValue("@Product_Count", product.Product_Count);
                    insertCmd.ExecuteNonQuery();
                }
                TempData["SuccessMessage"] = "Product added successfully!";
                return RedirectToAction("Create");
            }
            return View(product);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                // Redirect to the Index action if id is null
                return RedirectToAction("Index");
            }
            Product product = new Product();
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM products WHERE Product_Id = @Product_Id";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@Product_Id", id);
                adapter.Fill(dt);
            }
            if (dt.Rows.Count == 1)
            {
                product.Product_Id = Convert.ToInt32(dt.Rows[0]["Product_Id"]);
                product.Product_Name = dt.Rows[0]["Product_Name"].ToString();
                product.Product_Price = Convert.ToInt64(dt.Rows[0]["Product_Price"]);
                product.Product_Count = Convert.ToInt64(dt.Rows[0]["Product_Count"]);
                return View(product);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (product == null)
            {
                // Redirect to the Index action if id is null
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE products SET Product_Name = @Product_Name, Product_Price = @Product_Price, Product_Count = @Product_Count WHERE Product_Id = @Product_Id";
                    SqlCommand updateCmd = new SqlCommand(query, conn);
                    updateCmd.Parameters.AddWithValue("@Product_Id", product.Product_Id);
                    updateCmd.Parameters.AddWithValue("@Product_Name", product.Product_Name);
                    updateCmd.Parameters.AddWithValue("@Product_Price", product.Product_Price);
                    updateCmd.Parameters.AddWithValue("@Product_Count", product.Product_Count);
                    updateCmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            return View(product);
        }

        
        
        public ActionResult Delete(int? id)
        {
            if (id ==null)
            {
                return RedirectToAction("Index");
            }
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Index");

            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM products WHERE Product_Id = @Product_Id";
                SqlCommand deleteCmd = new SqlCommand(query, conn);
                deleteCmd.Parameters.AddWithValue("@Product_Id", id);
                deleteCmd.ExecuteNonQuery();
                return RedirectToAction("Index");
            }
            
        }
    }
}
