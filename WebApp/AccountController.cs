using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.Host;

namespace WebApp
{
    // TODO 4: unauthorized users should receive 401 status code
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize] 
        [HttpGet]
        public ValueTask<Account> Get()
        {
            //return _accountService.LoadOrCreateAsync(null /* TODO 3: Get user id from cookie */);
            var acc = _accountService.LoadOrCreateAsync((string)HttpContext.Request.Cookies["id"]);
            if (acc == null)
            {
                throw new HttpException((int)HttpStatusCode.Unauthorized, "Unauthorized");
            }

            return acc;
        }

        //TODO 5: Endpoint should works only for users with "Admin" Role
        [Authorize]
        [HttpGet("{id}")]
        public Account GetByInternalId([FromRoute] int id)
        {
            return _accountService.GetFromCache(id); 
        }

        [Authorize]
        [HttpPost("counter")]
        public async Task UpdateAccount()
        {
            //Update account in cache, don't bother saving to DB, this is not an objective of this task.
            var account = await Get();
            account.Counter++;
        }
    }
}