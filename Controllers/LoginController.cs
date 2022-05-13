using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElementsTheAPI.Filters;
using System.Net;
using ElementsTheAPI.Repositories;
using ElementsTheAPI.Models;

namespace ElementsTheAPI.Controllers
{
    [ApiKeyAuth]
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginRepository _repository;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILoginRepository repository, ILogger<LoginController> logger)
        {
            _repository = repository;
            _logger = logger;
        }


        [AllowAnonymous]
        [Route("register", Name = "Register")]
        [HttpPost]
        [ProducesResponseType(typeof(LoginResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<ActionResult<LoginResponse>> RegisterUser([FromBody] LoginRequest loginRequest)
        {
            return Ok(await _repository.RegisterUser(loginRequest));
        }

        [AllowAnonymous]
        [Route("login", Name = "Login")]
        [HttpPost]
        [ProducesResponseType(typeof(LoginResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<LoginResponse>> LoginUser([FromBody] LoginRequest loginRequest)
        {
            return Ok(await _repository.LoginUser(loginRequest));
        }

        [AllowAnonymous]
        [Route("version", Name = "Version")]
        [HttpPost]
        [ProducesResponseType(typeof(LoginResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<LoginResponse>> CheckAppVersion([FromBody] LoginRequest loginRequest)
        {
            return Ok(await _repository.CheckAppVersion(loginRequest));
        }
    }
}
