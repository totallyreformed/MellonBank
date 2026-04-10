using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MellonBank.Areas.Identity.Data;
using MellonBank.ViewModels;
using System.Security.Principal;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Build.Framework;

namespace MellonBank.Controllers
{
    public class StaffController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StaffController(AppDBContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Staff
        public IActionResult Index()
        {
            return View();
        }

        // Customer Operations

        // GET: Staff / Customers
        public async Task<IActionResult> Customers()
        {
            var users = await _userManager.GetUsersInRoleAsync("Customer");
            return View(users);
        }

        // GET: Staff / FindCustomer
        public async Task<IActionResult> FindCustomer()
        {
            return View();
        }

        // POST: Staff / FindCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FindCustomer(string afm)
        {
            if (string.IsNullOrEmpty(afm))
            {
                ModelState.AddModelError(string.Empty, "AFM is required.");
                return View();
            }

            var customer = await _context.Users.FirstOrDefaultAsync(u => u.AFM == afm);

            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "Customer not found.");
                return View();
            }

            return RedirectToAction(nameof(CustomerDetails), new { afm = afm });
        }

        // GET: Staff / CustomerDetails
        public async Task<IActionResult> CustomerDetails(string afm)
        {
            if (string.IsNullOrEmpty(afm))
            {
                return NotFound();
            }

            var customer = await _context.Users
                .Include(u => u.accounts)
                .FirstOrDefaultAsync(u => u.AFM == afm);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Staff / CreateCustomer
        public IActionResult CreateCustomer()
        {
            return View();
        }

        // POST: Staff / CreateCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Name = model.Name,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    Email = model.Email,
                    Address = model.Address,
                    PhoneNumber = model.Phone,
                    AFM = model.AFM
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    return RedirectToAction(nameof(Customers));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        // GET : Staff / EditCustomer
        public async Task<IActionResult> EditCustomer(string afm)
        {
            if (string.IsNullOrEmpty(afm))
            {
                return NotFound();
            }

            var customer = await _context.Users.FirstOrDefaultAsync(u => u.AFM == afm);
            if (customer == null)
            {
                return NotFound();
            }

            var model = new EditCustomerViewModel
            {
                Name = customer.Name,
                LastName = customer.LastName,
                Email = customer.Email,
                Address = customer.Address,
                Phone = customer.PhoneNumber,
                AFM = customer.AFM
            };

            return View(model);
        }

        // POST : Staff / EditCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(string afm, EditCustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = await _context.Users.FirstOrDefaultAsync(u => u.AFM == afm);

                if (customer == null)
                {
                    return NotFound();
                }

                customer.Name = model.Name;
                customer.LastName = model.LastName;
                customer.Address = model.Address;
                customer.PhoneNumber = model.Phone;
                customer.Email = model.Email;
                customer.AFM = model.AFM;

                _context.Update(customer);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Customers));
            }
            return View(model);
        }

        // GET: Staff / DeleteCustomer
        public async Task<IActionResult> DeleteCustomer(string afm)
        {
            if (string.IsNullOrEmpty(afm))
            {
                return NotFound();
            }

            var customer = await _context.Users
                .Include(u => u.accounts)
                .FirstOrDefaultAsync(u => u.AFM == afm);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Staff / DeleteCustomer
        [HttpPost, ActionName("DeleteCustomer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomerConfirmed(string afm)
        {
            var customer = await _context.Users
                .Include(u => u.accounts)
                .FirstOrDefaultAsync(u => u.AFM == afm);

            if (customer != null)
            {
                _context.Accounts.RemoveRange(customer.accounts);
                _context.Users.Remove(customer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Customers));
        }


        // Account Operations

        // GET: Staff / CreateAccount
        public IActionResult CreateAccount()
        {
            return View();
        }

        // POST: Staff / CreateAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = await _context.Users.FirstOrDefaultAsync(u => u.AFM == model.CustomerAFM);
                if (customer == null)
                {
                    ModelState.AddModelError("Customer AFM", "Customer with this AFM not found");
                    return View(model);
                }

                var account = new Account
                {
                    Balance = model.Balance,
                    AccountNumber = model.AccountNumber,
                    Branch = model.Branch,
                    Type = model.Type,
                    User = customer
                };

                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Customers));
            }
            return View(model);
        }

        // GET: Staff / EditAccount
        public async Task<IActionResult> EditAccount(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
            {
                return NotFound();
            }

            var model = new AccountViewModel
            {
                CustomerAFM = account.User.AFM,
                Balance = account.Balance,
                AccountNumber = account.AccountNumber,
                Branch = account.Branch,
                Type = account.Type
            };

            return View(model);
        }

        // POST: Staff / EditAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAccount(string accountNumber, AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var account = await _context.Accounts
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

                if (account == null)
                {
                    return NotFound();
                }

                account.Balance = model.Balance;
                account.AccountNumber = model.AccountNumber;
                account.Branch = model.Branch;
                account.Type = model.Type;

                _context.Update(account);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Customers));
            }

            return View(model);
        }

        // GET: Staff / DeleteAccount
        public async Task<IActionResult> DeleteAccount(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Staff / DeleteAccount
        [HttpPost, ActionName("DeleteAccount")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccountConfirmed(string accountNumber)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Customers));
        }
    }
}
