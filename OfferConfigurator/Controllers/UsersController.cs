using System;
using Microsoft.AspNetCore.Mvc;
using OfferConfigurator.Services;
using OfferConfigurator.Models;
using OfferConfigurator.Response;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfferConfigurator.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly VerifyHeaderId _verifyHeader;

        public UsersController(UserService userService, VerifyHeaderId verifyHeader)
        {
            _userService = userService;
            _verifyHeader = verifyHeader;
        }

        [HttpGet]
        public ActionResult<List<UserResult>> Get([FromHeader(Name = "X-HEADER-ID")][Required] string headerId)
        {
            if (headerId == null) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id not found", Data = new List<object>() });
            if (!_verifyHeader.checkUser(headerId)) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id incorrect", Data = new List<object>() });

            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Get all users", Data = _userService.Get() });
        }
        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public ActionResult<Catalog> Get([FromHeader(Name = "X-HEADER-ID")][Required] string headerId, string id)
        {
            if (headerId == null) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id not found", Data = new List<object>() });
            if (!_verifyHeader.checkUser(headerId)) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id incorrect", Data = new List<object>() });

            UserResult user = _userService.Get(id);

            if (user == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "User not found", Data = new List<object>() });
            }

            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Get a user", Data = user });
        }

        [HttpPut("{id:length(24)}")]
        public ActionResult<UserResult> Update([FromHeader(Name = "X-HEADER-ID")][Required] string headerId, string id, UserBody userBody)
        {
            if (headerId == null) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id not found", Data = new List<object>() });
            if (!_verifyHeader.checkUser(headerId)) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id incorrect", Data = new List<object>() });

            User user = _userService.GetById(id);

            if (user == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "User not found", Data = new List<object>() });
            }

            user.Email = userBody.Email;
            user.FirstName = userBody.FirstName;
            user.LastName = userBody.LastName;
            user.Password = user.Password;

            _userService.Update(id, user);

            UserResult result = _userService.UserFormat(user);

            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "User changed", Data = result });
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete([FromHeader(Name = "X-HEADER-ID")][Required] string headerId, string id)
        {
            if (headerId == null) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id not found", Data = new List<object>() });
            if (!_verifyHeader.checkUser(headerId)) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id incorrect", Data = new List<object>() });

            UserResult user = _userService.Get(id);

            if (user == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "User not found", Data = new List<object>() });
            }

            _userService.Remove(user.Id);

            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "User deleted", Data = new List<object>() });
        }
    }
}
