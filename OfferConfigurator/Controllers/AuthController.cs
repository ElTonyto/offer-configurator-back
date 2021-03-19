using System;
using Microsoft.AspNetCore.Mvc;
using OfferConfigurator.Services;
using OfferConfigurator.Models;
using OfferConfigurator.Response;
using System.Collections.Generic;

namespace OfferConfigurator.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public ActionResult<UserResult> Login(LoginBody loginBody)
        {
            User user = _authService.userService.GetByEmail(loginBody.Email);

            if (user == null)
            {
                return StatusCode(401, new HttpResponse { Status = 404, Type = "UNAUTHORIZED", Message = "Invalid credentials", Data = new List<object>() });
            }

            bool validPassword = _authService.CompareHash(loginBody.Password, user.Password);

            if (!validPassword)
            {
                return StatusCode(401, new HttpResponse { Status = 404, Type = "UNAUTHORIZED", Message = "Invalid credentials", Data = new List<object>() });
            }

            return StatusCode(200, new HttpResponse { Status = 201, Type = "CREATED", Message = "User created", Data = new { id = user.Id.ToString() } });
        }

        [HttpPost("register")]
        public ActionResult<UserResult> Register(UserBody userBody)
        {
            User alreadyExist = _authService.userService.GetByEmail(userBody.Email);

            if (alreadyExist != null)
            {
                return StatusCode(409, new HttpResponse { Status = 409, Type = "CONFLICT", Message = "User already exists", Data = new List<object>() });
            }

            string password = _authService.GetHash(userBody.Password);
            userBody.Password = password;

            UserResult user = _authService.Create(userBody);

            return StatusCode(201, new HttpResponse { Status = 201, Type = "CREATED", Message = "User created", Data = user });
        }
    }
}
