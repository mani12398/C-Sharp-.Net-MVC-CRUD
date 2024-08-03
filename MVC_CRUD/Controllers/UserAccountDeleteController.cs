using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC_CRUD.Controllers
{
    public class UserAccountDeleteController : Controller
    {
        readonly string connectionString = ConnectionString.DatabaseString.ConnectionString;
        // GET: UserAccountDelete
        public ActionResult DeleteAccount()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult DeleteAccountConfirmed()
        {
            try
            {
                // Retrieve current user's ID from session or any identifier
                int userId = Convert.ToInt32(Session["Id"]);

                // Log to console for debugging
                Console.WriteLine($"Deleting user with ID: {userId}");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Start a transaction to ensure both deletions succeed
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Delete user logins from UserLogins table
                            string deleteUserLoginsQuery = "DELETE FROM UserLogins WHERE UserId = @UserId";
                            SqlCommand deleteUserLoginsCmd = new SqlCommand(deleteUserLoginsQuery, conn, transaction);
                            deleteUserLoginsCmd.Parameters.AddWithValue("@UserId", userId);
                            deleteUserLoginsCmd.ExecuteNonQuery();

                            // Delete user from Users table
                            string deleteUserQuery = "DELETE FROM Users WHERE Id = @UserId";
                            SqlCommand deleteUserCmd = new SqlCommand(deleteUserQuery, conn, transaction);
                            deleteUserCmd.Parameters.AddWithValue("@UserId", userId);
                            deleteUserCmd.ExecuteNonQuery();

                            // Commit the transaction
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction if an error occurs
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }

                // Clear session or authentication cookies
                Session.Clear();
                FormsAuthentication.SignOut();

                // Redirect to login page after successful deletion
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                // Log the exception to console for debugging
                Console.WriteLine($"Error occurred: {ex.Message}");

                // Optionally, handle the exception or re-throw it for further handling
                throw;
            }
        }


    }
}