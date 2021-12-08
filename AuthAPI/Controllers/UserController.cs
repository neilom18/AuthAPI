using AuthAPI.DTO;
using AuthAPI.Entidade;
using AuthAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AuthAPI.Controllers
{
    [ApiController, Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public  IActionResult Get()
        {
            return Ok(_userService.Get());
        }
        [HttpGet, Route("{id}")]
        public IActionResult Get(Guid id)
        {
            return Ok(_userService.Get(id));
        }
        [HttpPost, AllowAnonymous]
        public IActionResult Cadastrar([FromBody]NewUserDTO newUserDTO)
        {
            return Created("", _userService.Create(
                new User
                {
                    Role = newUserDTO.Role,
                    Password = newUserDTO.Password,
                    Username = newUserDTO.Username,
                }));
        }

        [HttpPost,AllowAnonymous ,Route("login")]
        public IActionResult Login([FromBody]UserLoginDTO loginDTO)
        {
            return Ok(_userService.Login(loginDTO.Username, loginDTO.Password));
        }

        [HttpGet,Authorize ,Route("autenticado")]
        public string Autorizado() => $"autenticado {User.Identity.Name}";

        [HttpGet, Authorize(Roles = "admin"), Route("admin")]
        public string Admin() => $"autenticado admin";
    }
}
