using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MellonBank.Areas.Identity.Data;

namespace MellonBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly AppDBContext _context;

        public CurrencyController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Currency/GetRates
        [HttpGet("GetRates")]
        public async Task<ActionResult<Currency>> GetRates()
        {
            var currency = await _context.Currencies.FirstOrDefaultAsync();
            if (currency == null)
                return NotFound();
            return Ok(currency);
        }
    }
}
