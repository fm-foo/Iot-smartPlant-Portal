using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IoT_SmartPlant_Portal.Identity.Managers;
using IoT_SmartPlant_Portal.Identity.Models;
using IoT_SmartPlant_Portal.BindingModels;
using IoT_SmartPlant_Portal.JwtAuth;
using Microsoft.AspNetCore.Cors;

namespace IoT_SmartPlant_Portal.Controllers {

    [Route("[controller]")]
    [EnableCors("AllowAllHeaders")]
    [ApiController]
    public class AuthenticateController : ControllerBase {

        private readonly IJwtAuthService _jwtAuthService;
        private readonly MyUserManager<AppUser> _myUserManager;
        private readonly SignInManager<AppUser> _signInManager;

        /// <summary>
        /// AuthenticateController Construtor
        /// </summary>
        /// <param name="jwtAuthService"></param>
        /// <param name="userManger"></param>
        /// <param name="signInManager"></param>

        public AuthenticateController(IJwtAuthService jwtAuthService, MyUserManager<AppUser> myUserManger, SignInManager<AppUser> signInManager) {
            _jwtAuthService = jwtAuthService;
            _myUserManager = myUserManger;
            _signInManager = signInManager;
        }

        /// <summary>
        /// This method will atempt to Authenticate with the provided credentials
        /// </summary>
        /// <param name="model">user name and password from body</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        [AllowAnonymous]
        [HttpPost()]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticatePostBindingModel model, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            AppUser appUser = await _myUserManager.FindByEmailAsync(model.Email);

            if (appUser != null) {
                var result = await _signInManager.CheckPasswordSignInAsync(appUser, model.Password, false);
                if (result.Succeeded) {
                    string tokenString = _jwtAuthService.CreateToken(appUser.Id.ToString(), 24);
                    appUser.Token = tokenString;
                    appUser.PasswordHash = null;
                    return Ok(appUser);
                } else {
                    throw new Exception("Wrong password or username please try again."); // wrong password
                }
            }
            throw new Exception($"{model.Email} has not been registered, please register first.");
        }
    }
}