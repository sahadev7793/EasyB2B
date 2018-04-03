using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyB2B.DataAccess.Interfaces;
using EasyB2B.Helper;
using EasyB2B.Helper.Email;
using EasyB2B.Helper.EncryDecryHelper;
using EasyB2B.Helper.JWTTokenHelpers;
using EasyB2B.Models;
using EasyB2B.Models.API;
using EasyB2B.Models.Data;
using EasyB2B.Models.Helper.EmailHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EasyB2B.API.Controllers
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Controller
    {

        #region Global Declaration
        private readonly IUserProvider _userProvider;

        private readonly IOTPProvider _OTPProvider;

        private readonly IEmailHelper _IEmailHelper;

        private readonly IJWTTokenHelper _IJWTTokenHelper;

        private readonly IEncryptDecryptHelper _encryptDecryptHelper;

        #endregion

        #region Constructor
        public UserController(IUserProvider userProvider, IOTPProvider oTPProvider, IEmailHelper emailHelper, IJWTTokenHelper tokenHelper, IEncryptDecryptHelper encryptDecryptHelper)
        {
            _userProvider = userProvider;
            _OTPProvider = oTPProvider;
            _IEmailHelper = emailHelper;
            _IJWTTokenHelper = tokenHelper;
            _encryptDecryptHelper = encryptDecryptHelper;
        }

        #endregion

        #region Registration & Login APIs

        [HttpPost]
        [Route("/api/User/mregister")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> RegistrationByMobile([FromBody]string mobileNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(mobileNumber))
                {
                    return new BadRequestObjectResult("Mobile Number must be required");
                }

                var resultByMobileNumber = await _userProvider.GetByMobileNumberAsync(mobileNumber);
                if (resultByMobileNumber != null)
                {
                    if (resultByMobileNumber.Id != Guid.Empty)
                        return new BadRequestObjectResult("Mobile number already exits");
                }
                    User user = await AddNewUser(mobileNumber);
                    Guid userid = user.Id;

                    if (userid != Guid.Empty)
                    {
                        OTP objOTP = await InsertNewOTP(userid, "MSIGNUP");

                        if (objOTP.Id != Guid.Empty)
                        {
                            //TODO: send TOP from mobile call here
                            await _IEmailHelper.SendEmail(new EmailData { FromEmails = DefultValueHelper.DEFAULT_FROM_EMAIL, ToEmails = DefultValueHelper.DEFAULT_FROM_EMAIL, Subject = "Access code for APP", Body = objOTP.Code });
                        }
                    }
                    return new OkResult();
                }
                
            catch (Exception ex)
            {

                throw ex;
            }
        }

        
        [HttpPost]
        [Route("/api/User/register")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> Registration([FromBody]Registation registation)
        {
            try
            {
                if (string.IsNullOrEmpty(registation.Mobile) || string.IsNullOrEmpty(registation.Email) || string.IsNullOrEmpty(registation.Password))
                {
                    return new BadRequestObjectResult("Request not proper");
                }

                var resultByMobileNumber = await _userProvider.GetByMobileNumberAsync(registation.Mobile);
                if (resultByMobileNumber != null)
                {
                    if (resultByMobileNumber.Id != Guid.Empty)
                        return new BadRequestObjectResult("Mobile number already exits");
                }


                var resultByEmail = await _userProvider.GetByEmailAsync(registation.Email);
                if (resultByEmail != null)
                {
                    if (resultByEmail.Id != Guid.Empty)
                        return new BadRequestObjectResult("Email already exits");
                }

                User user = new User();
                user.MobileNumber = registation.Mobile;
                user.Email = registation.Email;
                user.Password = registation.Password;
                user.CreatedOn = DateTime.Now.ToUniversalTime();
                user.UpdatedOn = DateTime.Now.ToUniversalTime();
                user.CreatedBy = DefultValueHelper.DEFAULT_SYSTEMUSER;
                user.UpdatedBy = DefultValueHelper.DEFAULT_SYSTEMUSER;
                user.IsActive = false;
                await _userProvider.AddAsync(user);
                Guid userid = user.Id;

                if (userid != Guid.Empty)
                {
                    OTP objOTP = await InsertNewOTP(userid,"STS");

                    if (objOTP.Id != Guid.Empty && !string.IsNullOrEmpty(user.Email))
                    {
                        await _IEmailHelper.SendEmail(new EmailData { FromEmails = DefultValueHelper.DEFAULT_FROM_EMAIL, ToEmails = user.Email, Subject = "Access code for portal", Body = objOTP.Code });
                    }
                }
                return new OkResult();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        

        [HttpPost]
        [Route("/api/user/checkaccesscode")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> CheckAccessCode([FromBody] CheckAccessCode checkAccessCode)
        {
            if (string.IsNullOrEmpty(checkAccessCode.EmailOrMobile) && string.IsNullOrEmpty(checkAccessCode.AccessCode))
                return new BadRequestObjectResult("EmailOrMobile and AccessCode must be needed");
            else if (string.IsNullOrEmpty(checkAccessCode.EmailOrMobile))
                return new BadRequestObjectResult("EmailOrMobile  must be needed");
            else if (string.IsNullOrEmpty(checkAccessCode.AccessCode))
                return new BadRequestObjectResult("AccessCode  must be needed");
            User resultForUser = await GetUserDetailByEmailOrMobile(checkAccessCode.EmailOrMobile,checkAccessCode.IsMobileNumber);

            if (resultForUser == null)
                return new BadRequestObjectResult("User not exits");

            if (resultForUser.Id == Guid.Empty)
                return new BadRequestObjectResult("User not exits");



            var resultByUserIdAndAccessCode = await _OTPProvider.GetOTPDetailByUserIdAndAccessCodeAsync(resultForUser.Id, checkAccessCode.AccessCode);

            if (resultByUserIdAndAccessCode == null)
                return new BadRequestObjectResult("access code not found");

            if (resultByUserIdAndAccessCode.Id == Guid.Empty)
                return new BadRequestObjectResult("access code not found");

            if (resultByUserIdAndAccessCode.IsUsed)
                return new BadRequestObjectResult("access code already used");

            if (resultByUserIdAndAccessCode.ExpiryDateTime < DateTime.Now)
                return new BadRequestObjectResult("access code expried");

            resultByUserIdAndAccessCode.IsUsed = true;

            resultByUserIdAndAccessCode.UpdatedBy = DefultValueHelper.DEFAULT_SYSTEMUSER;

            resultByUserIdAndAccessCode.UpdatedOn = DateTime.Now.ToUniversalTime();

            var resultUpdateOTPDetails = await _OTPProvider.UpdateAsync(resultByUserIdAndAccessCode.Id, resultByUserIdAndAccessCode);

            if (!resultUpdateOTPDetails)
                return new BadRequestObjectResult("some problem in OTP update");

            var serilizeUser = _encryptDecryptHelper.Encrypt<User>(resultForUser);
            var JWTClaims = _IJWTTokenHelper.CraeateClaims("userdetails", serilizeUser);

           var token =  _IJWTTokenHelper.BuildToken(JWTClaims);

            return new OkObjectResult(token);
        }

        

        [HttpPost]
        [Route("/api/user/requstnewaccesscode")]
        public async Task<IActionResult> RequestForNewAccessCode([FromQuery] AccessCode accessCode)
        {
            if (string.IsNullOrEmpty(accessCode.EmailOrMobile))
                return new BadRequestObjectResult("EmailOrMobile must be needed");


            User resultForUser = await GetUserDetailByEmailOrMobile(accessCode.EmailOrMobile, accessCode.IsMobileNumber);

            if (resultForUser == null)
                return new BadRequestObjectResult("User not exits");

            if (resultForUser.Id == Guid.Empty)
                return new BadRequestObjectResult("User not exits");

            
            OTP objOTP = await InsertNewOTP(resultForUser.Id,"RA");

            if (objOTP.Id == Guid.Empty && string.IsNullOrEmpty(objOTP.Code))
                return new NotFoundObjectResult("OTP not insert successfully");
            
                return new OkObjectResult(objOTP.Code);
        }
        #endregion

        #region CRUD APIs

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), 200)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetNotes()
        {

            var result = await _userProvider.GetAllAsync();

            if (result == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(result);
        }




        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetNotesById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new BadRequestObjectResult("No Id input");
            }

            var result = await _userProvider.GetByIdAsync(id);

            if (result == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(result);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.MobileNumber) || string.IsNullOrEmpty(user.Email))
            {
                return new BadRequestObjectResult("content not found");
            }
            //user.CreatedBy = new Guid("87D1C2A5-1F1A-4E27-A100-08541C959350");
            //user.UpdatedBy = new Guid("87D1C2A5-1F1A-4E27-A100-08541C959350");
            await _userProvider.AddAsync(user);
            return new OkResult();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] User user)
        {
            if(user == null)
            {
                return new BadRequestResult();
            }
            if(id == Guid.Empty)
            {
                return new BadRequestObjectResult("Id not found");
            }
            var result = await _userProvider.UpdateAsync(id, user);

            return new OkObjectResult(result);
        }

        #endregion

        #region Private Methods
        private async Task<User> AddNewUser(string mobileNumber)
        {
            return await AddNewUser(mobileNumber, string.Empty, string.Empty);
        }

        private async Task<User> AddNewUser(string mobileNumber, string email, string password)
        {
            User user = new User();
            user.MobileNumber = mobileNumber;
            user.Email = email;
            user.Password = password;
            user.CreatedOn = DateTime.Now.ToUniversalTime();
            user.UpdatedOn = DateTime.Now.ToUniversalTime();
            user.CreatedBy = DefultValueHelper.DEFAULT_SYSTEMUSER;
            user.UpdatedBy = DefultValueHelper.DEFAULT_SYSTEMUSER;
            user.IsActive = false;
            await _userProvider.AddAsync(user);
            return user;
        }

        private async Task<User> GetUserDetailByEmailOrMobile(string emailOrMobile, bool isMobileNumber)
        {
            User resultForUser = new User();
            if (isMobileNumber)
            {
                resultForUser = await _userProvider.GetByMobileNumberAsync(emailOrMobile);
            }
            else
            {
                resultForUser = await _userProvider.GetByEmailAsync(emailOrMobile);
            }

            return resultForUser;
        }

        private async Task<OTP> InsertNewOTP(Guid userid, string type)
        {
            OTP objOTP = new OTP();
            objOTP.Code = OTPHelper.GetNewOTP();
            objOTP.UserId = userid;
            objOTP.IsUsed = false;
            objOTP.Type = type;
            objOTP.ExpiryDateTime = DefultValueHelper.DEFAULT_OTP_EXPRIRE_TIME;
            objOTP.CreatedBy = DefultValueHelper.DEFAULT_SYSTEMUSER;
            objOTP.CreatedOn = DateTime.Now.ToUniversalTime();
            objOTP.UpdatedBy = DefultValueHelper.DEFAULT_SYSTEMUSER;
            objOTP.UpdatedOn = DateTime.Now.ToUniversalTime();
            await _OTPProvider.AddAsync(objOTP);
            return objOTP;
        }
        #endregion
    }
}