using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyB2B.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace EasyB2B.API.Controllers
{
    [Produces("application/json")]
    [Route("api/ClearData")]
    public class ClearDataController : Controller
    {

        #region Global Declaration
        private readonly IUserProvider _userProvider;

        private readonly IOTPProvider _OTPProvider;

        #endregion

        #region Constructor
        public ClearDataController(IUserProvider userProvider, IOTPProvider oTPProvider)
        {
            _userProvider = userProvider;
            _OTPProvider = oTPProvider;
        }
        #endregion

        [HttpDelete]
        [ProducesResponseType(typeof(Task), 200)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> ClearData()
        {
            bool response = false;

            response = await _userProvider.RemoveAllAsync();

            response = await _OTPProvider.RemoveAllAsync();


            return new OkObjectResult(response);
        }


    }
}