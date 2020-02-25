using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BpmnKit.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MRS.Model;
using MRS.Service;
using MRS.Utils;
using Newtonsoft.Json;

namespace MRS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IFileService _fileService;

        public AccountController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IFileService fileService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _fileService = fileService;
        }


        //admin lấy role tạo tài khoản
        [HttpGet("Role")]
        public ActionResult GetRoles()
        {
            List<string> roleNames = new List<string>()
            { nameof(UserRoles.Admin)};
            var data = _roleManager.Roles.Where(_ => !roleNames.Contains(_.Name)).Adapt<List<BaseRoleVM>>();
            return Ok(data);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetAccountById(String id)
        {
            var data = await _userManager.FindByIdAsync(id);
            if (data == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(data);
            var result = data.Adapt<AccountVMById>();
            result.Roles = roles;

            return Ok(result);
        }
        [HttpGet]
        public ActionResult GetAllUsers()
        {
            var result = _userManager.Users.ToList().Adapt<List<AccountVM>>();
            return Ok(result);
        }

        [HttpGet("Profile")]
        [Authorize]
        public ActionResult GetProfile()
        {
            var user = _userManager.GetUserAsync(User).Result;
            //var result = user.Adapt<AccountVM>();
            //if (result.AcademicRank != null)
            //    result.AcademicRank = string.Concat(result.AcademicRank.Select(c => char.IsUpper(c) ? c.ToString() : ""));
            return Ok(user.Adapt<AccountVM>());
        }

        [HttpPost]
        public async Task<ActionResult> CreateAccount(AccountCM model)
        {
            //Check role
            List<string> roles = model.Roles.ToList();
            foreach (var i in roles)
            {
                //Not allow to add Admin
                if (i.ToLower().Equals(nameof(UserRoles.Admin).ToLower()))
                    return BadRequest("Role can not contains Admin");
            }
            try
            {
                User user = model.Adapt<User>();
                user.DateCreated = DateTime.Now;
                user.UserCreated = _userManager.GetUserAsync(User).Result.UserName;
                var currentUser = await _userManager.CreateAsync(user, model.Password);
                if (currentUser.Succeeded)
                {
                    await _userManager.AddToRolesAsync(user, model.Roles);
                    return StatusCode(201);
                }
                else
                {
                    return BadRequest(currentUser.Errors);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Role")]
        public async Task<ActionResult> CreateRolesAsync([FromBody]BaseRoleCM model)
        {
            try
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(201);
        }

        //create account with role. except admin

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> UpdateAccount(AccountUM model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();
                user = model.Adapt(user);
                user.DateUpdated = DateTime.Now;
                user.UserUpdated = user.UserName;
                var currentAccount = await _userManager.UpdateAsync(user);
                if (!currentAccount.Succeeded)
                {
                    return BadRequest(currentAccount.Errors);
                }
                var oldRole = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, oldRole);
                await _userManager.AddToRoleAsync(user, nameof(UserRoles.Customer));

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        [Authorize]
        [HttpPut("Password")]
        public async Task<ActionResult> UpdatePassword(PasswordVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return BadRequest();
            var result = await _userManager.ChangePasswordAsync(user, model.PrePassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("Avatar")]
        public async Task<ActionResult> GetAvatar()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Avatar == null) return NotFound();
            var result = await _fileService.GetFileAsync(user.Avatar);
            return File(result.Stream, result.ContentType);
        }

        [HttpGet("{id}/Avatar")]
        public async Task<ActionResult> GetAvatarById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.Avatar == null) return NotFound();
            var result = await _fileService.GetFileAsync(user.Avatar);
            return File(result.Stream, result.ContentType);
        }

        [HttpPut("Avatar")]
        public async Task<ActionResult> UpdateAvatar([FromForm]IFormFile avt)
        {
            var user = await _userManager.GetUserAsync(User);
            var oldAvt = user.Avatar;
            var path = await _fileService.SaveFile(FilePath.avatar, avt);
            user.Avatar = path;
            await _userManager.UpdateAsync(user);
            if (oldAvt != null) _fileService.DeleteFile(oldAvt);
            return Ok();
        }


        [HttpPut("Role")]
        public async Task<ActionResult> UpdateRoleAsync([FromBody]BaseRoleUM model)
        {
            try
            {
                var role = _roleManager.Roles.Where(_ => _.Id == model.Id.ToString()).FirstOrDefault();
                if (role == null) return NotFound();
                var result = await _roleManager.UpdateAsync(model.Adapt(role));
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }

    }
}
