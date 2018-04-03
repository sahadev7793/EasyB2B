using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyB2B.Helper.EncryDecryHelper;
using EasyB2B.Helper.JWTTokenHelpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyB2B.API.Controllers
{
    [Route("api/[controller]")]
    
    public class ValuesController : Controller
    { 
        private readonly IJWTTokenHelper _IJWTTokenHelper;

        private readonly IEncryptDecryptHelper _encryptDecryptHelper;

        public ValuesController(IJWTTokenHelper tokenHelper, IEncryptDecryptHelper encryptDecryptHelper)
        {
            _IJWTTokenHelper = tokenHelper;
            _encryptDecryptHelper = encryptDecryptHelper;
        }
        // GET api/values
        [HttpGet]
        [Authorize(Policy = "UserAuth")]
        public  IActionResult Get()
        {
            return new OkObjectResult("successfully done");
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]string value)
        {
            var str = _IJWTTokenHelper.ReadToken(value, "userdetails");

            if (_encryptDecryptHelper.TryDecrypt<Models.Data.User>(str, out Models.Data.User user))
            {
                return new OkObjectResult(user);
            }
            return new BadRequestResult();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
