using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using MVC_CRUD.Models;

namespace MVC_CRUD.Controllers
{
    public class SignUpController : Controller
    {
        readonly string connectionString = ConnectionString.DatabaseString.ConnectionString;

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(LoginSignUp model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isUsernameExist = IsUserExist(model.Username);
                    bool isEmailExist = IsEmailExist(model.Email);
                    bool isMobileExist = IsMobileExist(model.Mobile_No);

                    if (isUsernameExist || isEmailExist || isMobileExist)
                    {
                        List<string> errorMessages = new List<string>();

                        if (isUsernameExist)
                        {
                            errorMessages.Add("Username already exists.");
                        }

                        if (isEmailExist)
                        {
                            errorMessages.Add("Email already exists.");
                        }

                        if (isMobileExist)
                        {
                            errorMessages.Add("Mobile No already exists.");
                        }

                        // Add all error messages to ModelState
                        ModelState.AddModelError("", string.Join(" ", errorMessages));
                        return View(model);
                    }
                    else
                    {
                        // Encrypt sensitive data
                        string encryptedUsername = EncryptionHelper.Encrypt(model.Username);
                        string encryptedEmail = EncryptionHelper.Encrypt(model.Email);
                        string encryptedMobile = EncryptionHelper.Encrypt(model.Mobile_No);
                        string encryptedPassword = EncryptionHelper.Encrypt(model.UserPassword);

                        DateTime registrationDateTime = DateTime.Now;
                        

                        // Add user to the database with encrypted fields and registration date/time
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            string defaultRole = "Employee";
                            connection.Open();
                            string sql = "INSERT INTO Users (Username, Email, Mobile_No, UserPassword, RegistrationDate, RegistrationTime ,UserRole) " +
                                 "VALUES (@Username, @Email, @Mobile_No, @UserPassword, @RegistrationDate, @RegistrationTime, @UserRole)";
                            SqlCommand command = new SqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@Username", encryptedUsername);
                            command.Parameters.AddWithValue("@Email", encryptedEmail);
                            command.Parameters.AddWithValue("@Mobile_No", encryptedMobile);
                            command.Parameters.AddWithValue("@UserPassword", encryptedPassword);
                            command.Parameters.AddWithValue("@RegistrationDate", registrationDateTime.Date);
                            command.Parameters.AddWithValue("@RegistrationTime", registrationDateTime.TimeOfDay);
                            command.Parameters.AddWithValue("@UserRole", defaultRole);
                            // Execute the command and get the inserted UserId
                            command.ExecuteNonQuery();




                        }

                        TempData["SuccessMessage"] = "Registration successful. Please log in.";
                        return RedirectToAction("Register");
                    }
                }
                catch
                {
                    ModelState.AddModelError("", "An error occurred while processing your request. Please try again later.");
                    return View(model);
                }
            }

            return View(model);
        }

        private bool IsUserExist(string username)
        {
            string encryptedUsername = EncryptionHelper.Encrypt(username);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Username", encryptedUsername);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        private bool IsEmailExist(string email)
        {
            string encryptedEmail = EncryptionHelper.Encrypt(email);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Email", encryptedEmail);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        private bool IsMobileExist(string mobile_no)
        {
            string encryptedMobile = EncryptionHelper.Encrypt(mobile_no);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Users WHERE Mobile_No = @Mobile_No";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Mobile_No", encryptedMobile);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }
    }
}
