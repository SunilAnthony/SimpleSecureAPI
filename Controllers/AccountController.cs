using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections;
using JWT;
using JWT.Serializers;
using JWT.Algorithms;
using Microsoft.Extensions.Options;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;
using SimpleAPI.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SimpleAPI.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JWTSettings _options;
        private const int _tokenLength = 5;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IOptions<JWTSettings> optionsAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _options = optionsAccessor.Value;
        }
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] Credentials Credentials)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Credentials.Email, Credentials.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(Credentials.Email);
                    return new JsonResult(new Dictionary<string, object>
            {
                { "access_token", GetAccessToken(Credentials.Email) },
                { "username", user.Email },
                { "expired_on", DateTime.UtcNow.AddMinutes(_tokenLength) }, //Change to use UTC Time instead of .Now
                { "id_token", GetIdToken(user) } //Use to send to other application which container user information
            });
                }
                return new JsonResult("Invalid to sign in credential") { StatusCode = 401 };
            }
            return Error("Unexpected error");
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Credentials Credentials)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Credentials.Email, Email = Credentials.Email };
                var result = await _userManager.CreateAsync(user, Credentials.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return new JsonResult(new Dictionary<string, object>
                    {

                        { "access_token", GetAccessToken(Credentials.Email) },
                        { "id_token", GetIdToken(user) }
                     });
                }
                return Errors(result);

            }
            return Error("Unexpected error");
        }

        private string GetIdToken(ApplicationUser user)
        {
            var payload = new Dictionary<string, object>
            {
                { "id", user.Id },
                { "sub", user.Email },
                { "email", user.Email },
                { "emailConfirmed", user.EmailConfirmed },
            };
            return GetToken(payload);
        }

        private string GetAccessToken(string Email)
        {
            var payload = new Dictionary<string, object>
        {
            { "sub", Email },
            { "email", Email }
        };
            return GetToken(payload);
        }

        private string GetToken(Dictionary<string, object> payload)
        {
            var secret = _options.SecretKey;

            payload.Add("iss", _options.Issuer);
            payload.Add("aud", _options.Audience);
            payload.Add("nbf", ConvertToUnixTimestamp(DateTime.Now));
            payload.Add("iat", ConvertToUnixTimestamp(DateTime.Now));
            payload.Add("exp", ConvertToUnixTimestamp(DateTime.UtcNow.AddMinutes(_tokenLength))); //Change to use UTC time instead of .Now
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder.Encode(payload, secret);
        }

        private JsonResult Errors(IdentityResult result)
        {
            var items = result.Errors
                .Select(x => x.Description)
                .ToArray();
            return new JsonResult(items) { StatusCode = 400 };
        }

        private JsonResult Error(string message)
        {
            return new JsonResult(message) { StatusCode = 400 };
        }

        private static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
        
    }
}