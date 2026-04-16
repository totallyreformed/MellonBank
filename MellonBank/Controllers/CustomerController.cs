using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MellonBank.Areas.Identity.Data;
using MellonBank.ViewModels;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;

namespace MellonBank.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public CustomerController(AppDBContext context, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var accounts = await _context.Accounts
                .Where(a => a.User.Id == user.Id)
                .ToListAsync();

            return View(accounts);
        }

        // GET: Customer/AccountDetails/5
        public async Task<IActionResult> AccountDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id && a.User.Id == user.Id);

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Customer/Balance/5
        public async Task<IActionResult> Balance(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id && a.User.Id == user.Id);

            if (account == null)
            {
                return NotFound();
            }

            decimal balanceEur = account.Balance;
            decimal? balanceUsd = null;
            string errorMessage = null;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var apiKey = _configuration["FixerApi:ApiKey"];
                    var response = await httpClient.GetAsync($"https://data.fixer.io/api/latest?access_key={apiKey}&base=EUR&symbols=USD");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        using (var doc = JsonDocument.Parse(json))
                        {
                            var root = doc.RootElement;
                            if (root.GetProperty("success").GetBoolean())
                            {
                                var rate = root.GetProperty("rates").GetProperty("USD").GetDecimal();
                                balanceUsd = balanceEur * rate;
                            }
                        }
                    }
                }
            }
            catch
            {
                errorMessage = "Could not retrieve exchange rate.";
            }

            ViewBag.BalanceEur = balanceEur;
            ViewBag.BalanceUsd = balanceUsd;
            ViewBag.ErrorMessage = errorMessage;
            ViewBag.AccountNumber = account.AccountNumber;

            return View(account);
        }

        // GET: Customer/Deposit/5
        public async Task<IActionResult> Deposit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.User.Id == user.Id);

            if (account == null)
            {
                return NotFound();
            }

            ViewBag.AccountId = account.Id;
            ViewBag.AccountNumber = account.AccountNumber;
            ViewBag.CurrentBalance = account.Balance;

            return View();
        }

        // POST: Customer/Deposit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(int id, DepositViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.User.Id == user.Id);

            if (account == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (account.Balance + model.Amount > 9999999999999999.99m)
                {
                    ModelState.AddModelError("Amount", "This deposit would exceed the maximum allowed balance.");
                    ViewBag.AccountId = account.Id;
                    ViewBag.AccountNumber = account.AccountNumber;
                    ViewBag.CurrentBalance = account.Balance;
                    return View(model);
                }

                account.Balance += model.Amount;
                _context.Update(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AccountDetails), new { id = account.Id });
            }

            ViewBag.AccountId = account.Id;
            ViewBag.AccountNumber = account.AccountNumber;
            ViewBag.CurrentBalance = account.Balance;

            return View(model);
        }

        // GET: Customer/Transfer/5
        public async Task<IActionResult> Transfer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.User.Id == user.Id);

            if (account == null)
            {
                return NotFound();
            }

            ViewBag.AccountId = account.Id;
            ViewBag.AccountNumber = account.AccountNumber;
            ViewBag.CurrentBalance = account.Balance;

            return View();
        }

        // POST: Customer/Transfer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(int id, TransferViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.User.Id == user.Id);
            if (account == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (model.Amount > account.Balance)
                {
                    ModelState.AddModelError("Amount", "Insufficient funds. Your balance is " + account.Balance + " EUR.");
                    ViewBag.AccountId = account.Id;
                    ViewBag.AccountNumber = account.AccountNumber;
                    ViewBag.CurrentBalance = account.Balance;

                    return View(model);
                }

                var destinationAccount = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountNumber == model.DestinationAccountNumber);

                if (destinationAccount == null)
                {
                    ModelState.AddModelError("DestinationAccountNumber", "Destination account not found");
                    ViewBag.AccountId = account.Id;
                    ViewBag.AccountNumber = account.AccountNumber;
                    ViewBag.CurrentBalance = account.Balance;
                    return View(model);
                }

                if (destinationAccount.Balance + model.Amount > 9999999999999999.99m)
                {
                    ModelState.AddModelError("Amount", "This transfer would exceed the destination account's maximum allowed balance.");
                    ViewBag.AccountId = account.Id;
                    ViewBag.AccountNumber = account.AccountNumber;
                    ViewBag.CurrentBalance = account.Balance;
                    return View(model);
                }

                account.Balance -= model.Amount;
                destinationAccount.Balance += model.Amount;

                _context.Update(account);
                _context.Update(destinationAccount);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(AccountDetails), new { id = account.Id });
            }

            ViewBag.AccountId = account.Id;
            ViewBag.AccountNumber = account.AccountNumber;
            ViewBag.CurrentBalance = account.Balance;

            return View(model);
        }

        // GET: Customer/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Customer/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    ViewBag.SuccessMessage = "Password changed successfully";

                    return View();
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
    }
}