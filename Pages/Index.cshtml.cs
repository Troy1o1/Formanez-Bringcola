using Formanez_Bringcola.Data;
using Formanez_Bringcola.Helpers;
using Formanez_Bringcola.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Formanez_Bringcola.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Login Input Model
        [BindProperty]
        public LoginInputModel LoginInput { get; set; } = new LoginInputModel();

        // Register Input Model
        [BindProperty]
        public RegisterInputModel RegisterInput { get; set; } = new RegisterInputModel();

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Load messages from TempData
            if (TempData["Message"] != null)
            {
                var messageType = TempData["MessageType"]?.ToString();
                if (messageType == "success")
                {
                    SuccessMessage = TempData["Message"].ToString();
                }
                else
                {
                    ErrorMessage = TempData["Message"].ToString();
                }
            }
        }

        // LOGOUT HANDLER
        public IActionResult OnPostLogout()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            // Set success message
            TempData["Message"] = "You have been successfully logged out.";
            TempData["MessageType"] = "success";

            return RedirectToPage("/Index");
        }

        // LOGIN HANDLER
        public async Task<IActionResult> OnPostLoginAsync()
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(LoginInput.Username) ||
                    string.IsNullOrWhiteSpace(LoginInput.Password))
                {
                    ErrorMessage = "Please enter both username and password";
                    return Page();
                }

                // Find user
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == LoginInput.Username);

                if (user == null)
                {
                    ErrorMessage = "Invalid username or password";
                    return Page();
                }

                // Verify password
                if (!PasswordHasher.VerifyPassword(LoginInput.Password, user.PasswordHash))
                {
                    ErrorMessage = "Invalid username or password";
                    return Page();
                }

                // Check if active
                if (!user.IsActive)
                {
                    ErrorMessage = "Your account has been deactivated";
                    return Page();
                }

                // Update last login
                user.LastLoginAt = DateTime.Now;
                await _context.SaveChangesAsync();

                // Create session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("FirstName", user.FirstName ?? "");
                HttpContext.Session.SetString("LastName", user.LastName ?? "");

                // Redirect to products
                return RedirectToPage("/HomePage");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Login error: " + ex.Message;
                return Page();
            }
        }

        // REGISTER HANDLER
        public async Task<IActionResult> OnPostRegisterAsync()
        {
            try
            {
                // Clear any previous messages
                TempData.Remove("Message");
                TempData.Remove("MessageType");

                // Validate username
                if (string.IsNullOrWhiteSpace(RegisterInput.Username))
                {
                    TempData["ShowRegister"] = "true";
                    ErrorMessage = "Username is required";
                    return Page();
                }

                if (RegisterInput.Username.Length < 3)
                {
                    TempData["ShowRegister"] = "true";
                    ErrorMessage = "Username must be at least 3 characters";
                    return Page();
                }

                if (RegisterInput.Username.Length > 50)
                {
                    TempData["ShowRegister"] = "true";
                    ErrorMessage = "Username must be less than 50 characters";
                    return Page();
                }

                // Validate email
                if (string.IsNullOrWhiteSpace(RegisterInput.Email))
                {
                    TempData["ShowRegister"] = "true";
                    ErrorMessage = "Email is required";
                    return Page();
                }

                // Basic email validation
                if (!RegisterInput.Email.Contains("@") || !RegisterInput.Email.Contains("."))
                {
                    TempData["ShowRegister"] = "true";
                    ErrorMessage = "Please enter a valid email address";
                    return Page();
                }

                // Validate password
                if (string.IsNullOrWhiteSpace(RegisterInput.Password))
                {
                    TempData["ShowRegister"] = "true";
                    ErrorMessage = "Password is required";
                    return Page();
                }

                if (RegisterInput.Password.Length < 6)
                {
                    TempData["ShowRegister"] = "true";
                    ErrorMessage = "Password must be at least 6 characters";
                    return Page();
                }

                // Check password strength
                bool hasUppercase = false;
                bool hasLowercase = false;
                bool hasNumber = false;
                bool hasSpecial = false;

                foreach (char c in RegisterInput.Password)
                {
                    if (char.IsUpper(c)) hasUppercase = true;
                    if (char.IsLower(c)) hasLowercase = true;
                    if (char.IsDigit(c)) hasNumber = true;
                    if ("@$!%*?&".Contains(c)) hasSpecial = true;
                }

                if (!hasUppercase || !hasLowercase || !hasNumber || !hasSpecial)
                {
                    TempData["ShowRegister"] = "true";
                    ErrorMessage = "Password must contain uppercase, lowercase, number, and special character (@$!%*?&)";
                    return Page();
                }

                // Validate confirm password
                if (string.IsNullOrWhiteSpace(RegisterInput.ConfirmPassword))
                {
                    TempData["ShowRegister"] = "true";
                    ErrorMessage = "Please confirm your password";
                    return Page();
                }

                if (RegisterInput.Password != RegisterInput.ConfirmPassword)
                {
                    TempData["ShowRegister"] = "true";
                    ErrorMessage = "Passwords do not match";
                    return Page();
                }

                // Check if username already exists
                var existingUser = await _context.Users
                    .AnyAsync(u => u.Username == RegisterInput.Username);

                if (existingUser)
                {
                    TempData["ShowRegister"] = "true";
                    ErrorMessage = "Username already exists. Please choose a different username.";
                    return Page();
                }

                // Check if email already exists
                var existingEmail = await _context.Users
                    .AnyAsync(u => u.Email == RegisterInput.Email);

                if (existingEmail)
                {
                    TempData["ShowRegister"] = "true";
                    ErrorMessage = "Email already registered. Please use a different email.";
                    return Page();
                }

                // Create new user
                var newUser = new Users
                {
                    Username = RegisterInput.Username.Trim(),
                    Email = RegisterInput.Email.Trim(),
                    FirstName = string.IsNullOrWhiteSpace(RegisterInput.FirstName) ? null : RegisterInput.FirstName.Trim(),
                    LastName = string.IsNullOrWhiteSpace(RegisterInput.LastName) ? null : RegisterInput.LastName.Trim(),
                    PasswordHash = PasswordHasher.HashPassword(RegisterInput.Password),
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                // Add to database
                _context.Users.Add(newUser);

                // Save changes
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    // Success - clear the ShowRegister flag
                    TempData.Remove("ShowRegister");

                    // Set success message
                    TempData["Message"] = "Account created successfully! Please login with your credentials.";
                    TempData["MessageType"] = "success";

                    return RedirectToPage("/Index");
                }
                else
                {
                    TempData["ShowRegister"] = "true";
                    ErrorMessage = "Failed to save user to database. Please try again.";
                    return Page();
                }
            }
            catch (DbUpdateException dbEx)
            {
                TempData["ShowRegister"] = "true";
                ErrorMessage = "Database error: " + (dbEx.InnerException?.Message ?? dbEx.Message);
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ShowRegister"] = "true";
                ErrorMessage = "Error: " + ex.Message;
                return Page();
            }
        }
    }

    // Input Models
    public class LoginInputModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterInputModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}