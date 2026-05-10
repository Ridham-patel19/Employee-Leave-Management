using Microsoft.AspNetCore.Mvc;
using MVC;

namespace MVC.Namespace
{
    public class AuthController : Controller
    {
        // 
        // 
        // GET: AuthController

        private readonly IAuthInterface _authRepo;
        private readonly IWebHostEnvironment _env;
        public AuthController(IAuthInterface auth , IWebHostEnvironment env)
        {
            _authRepo = auth;
            _env = env;
        }

        public IActionResult Login()
    {

        HttpContext.Session.Clear();
        return View();
    }

    public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

    public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] User user)
        {

            System.Console.WriteLine(user.ProfilePic);
            if(user == null)
            {
                return BadRequest(new
                {
                    success = false ,
                    message = "eneter valid credential"
                });
            }
                     
                     System.Console.WriteLine("here");
            if(user.ProfilePic != null && user.ProfilePic.Length > 0)
            {
                string imageurl = Guid.NewGuid() + Path.GetExtension(user.ProfilePic.FileName);

                System.Console.WriteLine(imageurl);

                string folderpath = Path.Combine(_env.WebRootPath , "ProfileImages");

                System.Console.WriteLine(folderpath);

                if (!Directory.Exists(folderpath))
                {
                    System.Console.WriteLine("inside directory if");
                    Directory.CreateDirectory(folderpath);
                }

                string filepath = Path.Combine(folderpath , imageurl);


                using (var stream = new FileStream(filepath, FileMode.Create))
{
    user.ProfilePic.CopyTo(stream);
}


user.Image = imageurl;

            }
            

            int result = await _authRepo.Register(user);

                if(result == 1)
            {
                return Ok(new
                {
                    success = true,
                    message = "Inserted Successfully"
                });


               
                
            }
            else
            {
                return BadRequest(new
                {
                    success = true,
                    message = "Error in insertion"
                });
            }



          
        }

[HttpPost]
public async Task<IActionResult> Login([FromForm] vm_login login)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(new
        {
            success = false,
            message = "Please enter valid data"
        });
    }

    User user = await _authRepo.Login(login);

    if (user == null)
    {
        return BadRequest(new
        {
            success = false,
            message = "Invalid Credential"
        });
    }

    HttpContext.Session.SetString("role", user.Role);

    HttpContext.Session.SetInt32("UserId", user.UserId);

    return Ok(new
    {
        success = true,
        role = user.Role
    });
}
    }
}
