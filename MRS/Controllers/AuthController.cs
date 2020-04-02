using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MRS.BpmnViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MRS.Model;
using MRS.Utils;
using Newtonsoft.Json;

namespace MRS.PhapYControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public AuthController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public ActionResult Get()
        {
            return Ok();
        }
        [HttpPost("token")]
        public async Task<ActionResult> GetToken([FromBody]LoginMobiVM model)
        {
            User user;
            if (model.Account_Id != null)
            {
                user = _userManager.Users.Where(u => u.Account_Id == model.Account_Id).FirstOrDefault();
                if (user == null) return BadRequest(new { Message = "Invalid Account_Id" });
            }
            else
            {
                user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    return BadRequest(new { Message = "Invalid Username" });
                }
                var result = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!result)
                {
                    return BadRequest(new { Message = "Invalid Password" });
                }
            }
            return new OkObjectResult(GenerateToken(user).Result);
        }

        private async Task<Token> GenerateToken(User user)
        {
            //security key
            string securityKey = "qazedcVFRtgbNHYujmKIolp";
            //symmectric security key
            var symmectricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            //signing credentials
            var signingCredentials = new SigningCredentials(symmectricSecurityKey, SecurityAlgorithms.HmacSha256);

            //add Claims
            var claims = new List<Claim>();
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            //create token
            var token = new JwtSecurityToken(
                    issuer: "huyng",
                    audience: user.FullName,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: signingCredentials,
                    claims: claims
                );
            //return token
            return new Token
            {
                roles = _userManager.GetRolesAsync(user).Result.ToArray(),
                fullname = user.FullName,
                score = user.Score,
                rank = GetUserLevelString(user.Level),
                Device_Id = user.Device_Id,
                access_token = new JwtSecurityTokenHandler().WriteToken(token),
                expires_in = (int)TimeSpan.FromHours(1).TotalSeconds
            };
        }

        public string GetUserLevelString(int i)
        {
            if (i == (int)UserLevel.Gold)
            {
                return nameof(UserLevel.Gold);
            }
            else if (i == (int)UserLevel.Member)
            {
                return nameof(UserLevel.Member);
            }
            else
            {
                return nameof(UserLevel.Platinum);
            }
        }
        //tự đăng kí
        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody]RegisterMobiVM model)
        {
            try
            {
                User user;
                string password;
                if (model.Username == null && model.Password == null && model.Account_Id == null)
                {
                    return BadRequest(new { Message = "UserName,Password,Account_ID???" });
                }
                if (model.Username == null && model.Password == null)
                {
                    user = new User()
                    {
                        UserName = model.Email + DateTime.Now.Date.ToShortDateString(),
                        Email = model.Email,
                        FullName = model.Fullname,
                        Account_Id = model.Account_Id,
                        Device_Id = model.Device_Id
                    };
                    password = model.Email + DateTime.Now.Date.ToShortDateString();
                }
                else
                {
                    user = new User()
                    {
                        UserName = model.Username,
                        Email = model.Email,
                        FullName = model.Fullname,
                        Account_Id = model.Account_Id,
                        Device_Id = model.Device_Id
                    };
                    password = model.Password;
                }
                user.DateCreated = DateTime.Now;
                user.UserCreated = model.Fullname;
                var resultUser = await _userManager.CreateAsync(user, password);
                var resultRole = await _userManager.AddToRoleAsync(user, nameof(UserRoles.Customer));
                if (resultUser.Succeeded)// && resultRole.Succeeded)
                {
                    return new OkObjectResult(GenerateToken(user).Result);
                }
                else
                {
                    return BadRequest(new { Message = resultUser.Errors });// + " \n" + resultRole.Errors);
                }

            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

    }
}