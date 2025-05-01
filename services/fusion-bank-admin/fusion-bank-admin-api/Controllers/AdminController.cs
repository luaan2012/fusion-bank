using fusion.bank.admin.domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fusion.bank.admin.api.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class CentralBankController(IDashboardServices dashboardServices) : MainController
    {

        [HttpGet("dashboard-admin")]
        public async Task<IActionResult> CreateBank()
        {
            return Ok();
        }
    }
}
