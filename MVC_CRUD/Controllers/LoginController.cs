using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MVC_CRUD.Models;

namespace MVC_CRUD.Controllers
{
    public class LoginController : Controller
    {
        readonly string connectionString = ConnectionString.DatabaseString.ConnectionString;

        [AllowAnonymous]
        public ActionResult Login()
        {
            // Ensure the login page is not cached by browsers
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetNoStore();

            // Check if user is already authenticated
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to Product/Index if authenticated
                return RedirectToAction("Index", "Product");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(LoginSignUp model, bool rememberMe = false)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string encryptedIdentifier;
                    bool isEmail = model.Username.Contains("@"); // Check if input is email format

                    // Encrypt username or email
                    encryptedIdentifier = isEmail ? EncryptionHelper.Encrypt(model.Username) : EncryptionHelper.Encrypt(model.Username);

                    // Encrypt password
                    string encryptedPassword = EncryptionHelper.Encrypt(model.UserPassword);

                    // Validate user credentials
                    bool isValid = ValidateUser(encryptedIdentifier, encryptedPassword, isEmail);

                    if (isValid)
                    {
                        // Retrieve user details based on identifier type
                        User loggedInUser = isEmail ? GetUserDetailsByEmail(encryptedIdentifier) : GetUserDetailsByUsername(encryptedIdentifier);

                        // Store user information in Session
                        Session["Id"] = loggedInUser.Id;
                        Session["Username"] = loggedInUser.Username;
                        Session["Email"] = loggedInUser.Email;
                        Session["Mobile_No"] = loggedInUser.Mobile_No;
                        Session["UserRole"] = loggedInUser.UserRole;

                        // Log login date and time
                        LogLoginTime(loggedInUser.Id);

                        // Create authentication ticket
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                            1,                              // Ticket version
                            model.Username,                 // Username associated with the ticket
                            DateTime.Now,                   // Date/time issued
                            rememberMe ? DateTime.Now.AddDays(7) : DateTime.Now.AddMinutes(30), // Expiry date/time
                            rememberMe,                     // Persistent cookie?
                            "user",                         // User data
                            FormsAuthentication.FormsCookiePath); // Cookie path

                        // Encrypt the ticket
                        string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                        // Create the cookie
                        HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        if (ticket.IsPersistent)
                        {
                            authCookie.Expires = ticket.Expiration;
                        }
                        Response.Cookies.Add(authCookie);

                        // Redirect to the originally requested URL, if any
                        string returnUrl = Request.QueryString["ReturnUrl"];
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Product");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username, email, or password.");
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception and handle appropriately
                    ModelState.AddModelError("", "An error occurred while logging in. Please try again later.");
                    // Log the exception for debugging or logging purposes
                    Console.WriteLine($"Error occurred during login: {ex.Message}");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        private bool ValidateUser(string encryptedIdentifier, string encryptedPassword, bool isEmail)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql;

                if (isEmail)
                {
                    sql = "SELECT COUNT(*) FROM Users WHERE Email = @Identifier AND UserPassword = @UserPassword";
                }
                else
                {
                    sql = "SELECT COUNT(*) FROM Users WHERE Username = @Identifier AND UserPassword = @UserPassword";
                }

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Identifier", encryptedIdentifier);
                command.Parameters.AddWithValue("@UserPassword", encryptedPassword);

                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        private User GetUserDetailsByUsername(string encryptedUsername)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT Id, Username, Email, Mobile_No, UserRole FROM Users WHERE Username = @Username";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Username", encryptedUsername);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string decryptedUsername = EncryptionHelper.Decrypt(reader.GetString(1));
                    string decryptedEmail = EncryptionHelper.Decrypt(reader.GetString(2));
                    string decryptedMobile_No = EncryptionHelper.Decrypt(reader.GetString(3));
                    string userRole = reader.GetString(4);

                    return new User
                    {
                        Id = reader.GetInt32(0),
                        Username = decryptedUsername,
                        Email = decryptedEmail,
                        Mobile_No = decryptedMobile_No,
                        UserRole = userRole,
                    };
                }
            }

            return null; // Handle no user found case
        }

        private User GetUserDetailsByEmail(string encryptedEmail)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT Id, Username, Email, Mobile_No, UserRole FROM Users WHERE Email = @Email";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Email", encryptedEmail);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {

                    string decryptedUsername = EncryptionHelper.Decrypt(reader.GetString(1));
                    string decryptedEmail = EncryptionHelper.Decrypt(reader.GetString(2));
                    string decryptedMobile_No = EncryptionHelper.Decrypt(reader.GetString(3));
                    string userRole = reader.GetString(4);

                    return new User
                    {
                        Id = reader.GetInt32(0),
                        Username = decryptedUsername,
                        Email = decryptedEmail,
                        Mobile_No = decryptedMobile_No,
                        UserRole = userRole,
                    };
                }
            }

            return null; // Handle no user found case
        }


        private void LogLoginTime(int userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO UserLogins (UserId, LoginDate, LoginTime) VALUES (@UserId, @LoginDate, @LoginTime)";
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@LoginDate", DateTime.Now.Date);
                command.Parameters.AddWithValue("@LoginTime", DateTime.Now.TimeOfDay);

                command.ExecuteNonQuery();
            }
        }
        private void LogLogoutTime(int UserId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Update the most recent login entry with the logout time
                string sql = @"UPDATE UserLogins SET LogoutDate = @LogoutDate, LogoutTime = @LogoutTime WHERE UserId = @UserId AND LogoutDate IS NULL AND LogoutTime IS NULL";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@UserId", UserId);
                command.Parameters.AddWithValue("@LogoutDate", DateTime.Now.Date);
                command.Parameters.AddWithValue("@LogoutTime", DateTime.Now.TimeOfDay);

                command.ExecuteNonQuery();
            }
        }

        public ActionResult Logout()
        {
            int UserId = Convert.ToInt32(Session["Id"]);

            // Log the logout time
            LogLogoutTime(UserId);
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Login");
        }
    }
}