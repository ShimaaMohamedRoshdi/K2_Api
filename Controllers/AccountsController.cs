using AccountDocApi.Data;
using AccountDocApi.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AccountDocApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves Account and Customer information for a given Account Number.
        /// </summary>
        /// <param name="accountNo">The Account Number to look up (e.g. ACC1001)</param>
        /// <returns>Account details mapped for Nintex integration</returns>
        [HttpGet("{accountNo}")]
        [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountDto>> GetAccountByNo(string accountNo)
        {
            var account = await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountNo == accountNo);

            if (account == null)
            {
                return NotFound(new { message = $"Account '{accountNo}' was not found." });
            }

            var dto = new AccountDto
            {
                AccountNo = account.AccountNo,
                CustomerId = account.CustomerId,
                CustomerName = account.CustomerName,
                AccountType = account.AccountType,
                AccountStatus = account.AccountStatus,
                Branch = account.Branch
            };

            return Ok(dto);
        }
    }
}
