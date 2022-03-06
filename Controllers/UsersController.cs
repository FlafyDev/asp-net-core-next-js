using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Boxing.Controllers
{
	[Route("api/[controller]")]
	public class UsersController : Controller
	{
		/*[HttpGet("[action]")]
		public string Test(string username)
		{
			if (HttpContext.Session.Get("Halo") == null)
			{
				HttpContext.Session.SetInt32("Halo", 1);
			}
			else
			{
				HttpContext.Session.SetInt32("Halo", (int)HttpContext.Session.GetInt32("Halo") + 1);
			}

			return HttpContext.Session.GetInt32("Halo").ToString();
		}*/

		private bool IsAdmin()
		{
			return IsLoggedIn() && HttpContext.Session.GetInt32("admin") == 1;
		}

		private bool IsLoggedIn()
		{
			return HttpContext.Session.GetString("username")?.Length > 0;
		}

		[HttpGet("[action]")]
		public ActionResult<string> GetData()
		{
			if (!IsLoggedIn()) return StatusCode(401);

			return StatusCode(200, JsonConvert.SerializeObject(new
			{
				username = HttpContext.Session.GetString("username"),
				isAdmin = IsAdmin(),
			}));
		}

		[HttpGet("[action]")]
		public ActionResult<UserHandler.User[]> GetUsers()
		{
			if (!IsAdmin()) return StatusCode(401);
			return StatusCode(200, JsonConvert.SerializeObject(UserHandler.GetUsers("select * from Users")));
		}

		[HttpGet("[action]")]
		public IActionResult Logout()
		{
			if (!IsLoggedIn()) return StatusCode(401);
			HttpContext.Session.SetString("username", "");
			HttpContext.Session.SetInt32("admin", 0);
			return StatusCode(200);
		}
		
		[HttpPost("[action]")]
		public IActionResult UpdateUser([FromBody] UserHandler.User newUserDetails)
		{
			if (!IsAdmin()) return StatusCode(401);

			UserHandler.UpdateUser(newUserDetails);

			return StatusCode(200);
		}

		[HttpPost("[action]")]
		public IActionResult DeleteUser([FromBody] UserHandler.User user)
		{
			if (!IsAdmin()) return StatusCode(401);

			UserHandler.DeleteUser(user.Id);

			return StatusCode(200);
		}

		[HttpPost("[action]")]
		public IActionResult Login([FromBody] UserHandler.User userLoginDetails)
		{
			Logout();

			var user = UserHandler.GetUserByUsername(userLoginDetails?.Username);
			if (user != null && user.Password == userLoginDetails.Password)
			{
				HttpContext.Session.SetString("username", userLoginDetails.Username);
				HttpContext.Session.SetInt32("admin", user.Admin ? 1 : 0);
				return StatusCode(200);
			}
			return StatusCode(401);
		}

		[HttpPost("[action]")]
		public IActionResult Register([FromBody] UserHandler.User userRegisterDetails)
		{
			var user = UserHandler.RegisterUser(userRegisterDetails, out string error);
			if (error != "")
			{
				return StatusCode(400, error);
			}
			return StatusCode(200, user.Username);
		}
	}
}
