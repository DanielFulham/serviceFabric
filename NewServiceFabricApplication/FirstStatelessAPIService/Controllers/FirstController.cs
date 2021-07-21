using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstStatelessAPIService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FirstController : ControllerBase
    {
        [HttpGet]
        [Route("get")]
        public async Task Get()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
