using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendEmailService.Data;
using SendEmailService.Data.Model;
using SendEmailService.Models;
using SendEmailService.Service;

namespace SendEmailService.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AdminController(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        // Display the list of users
        public async Task<IActionResult> Index()
        {
            var users = await dbContext.Users
            .Select(user => new UserFormViewModel
            {
                Id = user.Id,
                Email = user.Email,
            })
            .ToListAsync();
            return View(users);
        }

        // Show the create user form
        public IActionResult Create()
        {
            var model = new UserFormViewModel();
            return View(model);
        }

        // Process the create user form
        [HttpPost]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            var entity = new User
            {
                Email = model.Email,
                TempPassword = EmailService.GenerateTemporaryPassword()
            };

            await dbContext.Users.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Show the edit user form
        public async Task<IActionResult> Edit(int id)
        {
            var model = await dbContext.Users
                .Where(user => user.Id == id)
                .AsNoTracking()
                .Select(user => new UserFormViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                })
                .FirstOrDefaultAsync();

            return View(model);
        }

        // Process the edit user form
        [HttpPost]
        public async Task<IActionResult> Edit(UserFormViewModel model,int id)
        {
            var entity = await dbContext.Users.FindAsync(id);

            entity.Email = model.Email;

            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Delete a user
        public async Task<IActionResult> Delete(int id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user != null)
            {
                dbContext.Users.Remove(user);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}

