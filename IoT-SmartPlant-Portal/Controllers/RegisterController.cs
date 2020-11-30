using System;
using System.Threading;
using System.Threading.Tasks;
using IoT_SmartPlant_Portal.BindingModels;
using IoT_SmartPlant_Portal.Identity.Managers;
using IoT_SmartPlant_Portal.Identity.Models;
using Microsoft.AspNetCore.Mvc;

namespace IoT_SmartPlant_Portal.Controllers {

    [Route("[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase {
        private readonly MyUserManager<AppUser> _userManager;

        /// <summary>
        /// Register Controller contructor
        /// </summary>
        /// <param name="emailService"></param>
        /// <param name="userManager"></param>
        public RegisterController(MyUserManager<AppUser> userManager) {
            _userManager = userManager;
        }

        /// <summary>
        /// Create a new user and send E-mail with activation link.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> Register([FromBody] RegisterPostBindingModel model, string id, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            AppUser appUser = new AppUser(model.Email);

            appUser.Devices.Add(Guid.Parse(id));

            var result = await _userManager.CreateUserAsync(appUser, model.Password);

            if (!result) {
                return Ok($"{model.Email} has been register succesfully");
            }
            return BadRequest("something has gone wrong check if email is correct");

        }


    }
}