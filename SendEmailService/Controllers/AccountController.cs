using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendEmailService.Data;
using SendEmailService.Models;
using SendEmailService.Service;

namespace SendEmailService.Controllers
{
    public class AccountController : Controller
    {
        private readonly EmailService _emailService;
        private readonly ApplicationDbContext dbContext; // Your database context

        public AccountController(EmailService emailService, ApplicationDbContext _dbContext)
        {
            _emailService = emailService;
            dbContext = _dbContext;
        }

        public async Task<IActionResult> Login()
        {
            var model = new UserLoginModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the email exists in the database
                var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == model.Email);

                if (user != null)
                {
                    // Generate a temporary password
                    var tempPassword = user.TempPassword;

                    // Save the temporary password securely (e.g., hashed in the database)
                    user.TempPassword = tempPassword; // Assuming you have a TempPassword field
                    await dbContext.SaveChangesAsync();

                    // Send email with temporary password
                    await _emailService.SendEmailAsync(
                        toEmail: model.Email,
                        subject: "Your Temporary Password",
                        body: $"Hello, your temporary password is: {tempPassword}"
                    );

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(nameof(UserLoginModel), "Email not found."); // Optional: inform user email is not registered
                }
            }

            // If the email does not exist or ModelState is invalid, return to the login view
            return View(model);
        }
    }
}
