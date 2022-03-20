using ElementsTheAPI.Filters;
using ElementsTheAPI.Models;
using ElementsTheAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ElementsTheAPI.Controllers
{
    [Authorize]
    [ApiKeyAuth]
    [ApiController]
    [Route("[controller]")]
    public class UserDataController : ControllerBase
    {
        private readonly IUserDataRepository _repository;
        private readonly ILogger<UserDataController> _logger;


        public UserDataController(IUserDataRepository repository, ILogger<UserDataController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [Route("save-game", Name = "SaveGame")]
        [HttpPost]
        [ProducesResponseType(typeof(AccountResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<AccountResponse>> SaveGameData([FromBody] AccountRequest accountRequest)
        {
            return Ok(await _repository.SaveUserData(accountRequest));
        }

        [Route("update-account", Name = "Email")]
        [HttpPost]
        [ProducesResponseType(typeof(AccountResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<AccountResponse>> ChangeEmail([FromBody] AccountRequest accountRequest)
        {
            return Ok(await _repository.UpdateUserDetails(accountRequest));
        }

        [Route("reset-account", Name = "ResetAccount")]
        [HttpPost]
        [ProducesResponseType(typeof(AccountResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<AccountResponse>> ResetAccount([FromBody] AccountRequest accountRequest)
        {
            return Ok(await _repository.ResetUserData(accountRequest));
        }

        [Route("refresh-token", Name = "TokenRefresh")]
        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<string>> RefreshToekn()
        {
            return Ok(await _repository.RefreshToken(Request.Headers[HeaderNames.Authorization]));
        }
    }
}
