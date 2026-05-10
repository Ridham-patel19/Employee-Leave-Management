using Microsoft.AspNetCore.Mvc;
using MVC.Models;

namespace MVC.Namespace
{
    public class EmployeeController : Controller
    {


        private readonly IEmpInterface _empRepo;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(IEmpInterface empRepo, IWebHostEnvironment env)
        {
            _empRepo = empRepo;
            _env = env;
        }
        // GET: EmployeeController
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Form()
        {
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            int? empid = HttpContext.Session.GetInt32("UserId");
            System.Console.WriteLine(empid + " hello ");
            if (empid == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            User? user = await _empRepo.GetEmpById(empid.Value);
            if (user == null)
            {
                return RedirectToAction("Dashboard");
            }

            return View(user);
        }


[HttpPost]
        public async Task<IActionResult> CreateApplication([FromBody]Application app)
        {
            if(app == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "please eneter valid credentials.."
                });
            }

        int? empid = HttpContext.Session.GetInt32("UserId");

        if(empid == null)
            {
                return RedirectToAction("Login" , "Auth");
            }
            int leftleaves = await _empRepo.LeftLeaveByEmp(app.LeaveId , empid);

            if(leftleaves <= 0)
            {
                return BadRequest(new
                {
                    success = false ,
                    message = "You have taken all your leavs"
                });
            }
            else
            {
                int result = await _empRepo.Create(app);

                if(result == 0)
                {
                    return BadRequest(new{
                        success = false ,
                        message = "Erro while applying leave"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = true ,
                        message = "Applied sccess fully"
                    });
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateApplication([FromBody]Application app)
        {
            if(app == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "please eneter valid credentials.."
                });
            }

        int? empid = HttpContext.Session.GetInt32("UserId");

        if(empid == null)
            {
                return RedirectToAction("Login" , "Auth");
            }

            Application? existingApplication = await _empRepo.GetApplicationById(app.ApplicationId ?? 0, empid);
            if (existingApplication == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Application not found"
                });
            }

            if (existingApplication.StartDate <= DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "You can update application only before the start date"
                });
            }

            app.EmpId = empid;
            int leftleaves = await _empRepo.LeftLeaveByEmp(app.LeaveId , empid);

            if(leftleaves <= 0)
            {
                return BadRequest(new
                {
                    success = false ,
                    message = "You have taken all your leavs"
                });
            }
            else
            {
                int result = await _empRepo.Update(app);

                if(result == 0)
                {
                    return BadRequest(new{
                        success = false ,
                        message = "Erro while Updating leave"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = true ,
                        message = "Updated sccess fully"
                    });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteApplication([FromBody]int id)
        {
            int? empid = HttpContext.Session.GetInt32("UserId");
            if (empid == null)
            {
                return BadRequest(1);
            }

            Application? existingApplication = await _empRepo.GetApplicationById(id, empid);
            if (existingApplication == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Application not found"
                });
            }

            if (existingApplication.EndDate >= DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "You can delete application only after the ending date"
                });
            }

            int result = await _empRepo.Delete(id);
            if (result == 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error while deleting application"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Application deleted successfully"
            });
        }


        public async Task<IActionResult> GetAllByEmp()
        {
            int? empid = HttpContext.Session.GetInt32("UserId");
            System.Console.WriteLine(empid);

        if(empid == null)
            {
                return RedirectToAction("Login", "Auth");
            }
          
            List<Application> list = new List<Application>();

            list = await _empRepo.GetAllById(empid);

            if(list == null)
            {
                return BadRequest(new
                {
                    success = false ,
                    message = "Eroor while getiing all application"
                });
            }
            else
            {
                return Ok(list);
            }
        }

        public  async Task<IActionResult> GetLeftLeave([FromForm]int typeid)
        {
            int? empid = HttpContext.Session.GetInt32("UserId");

        if(empid == null)
            {
                return BadRequest(1);
            }

            int result = await _empRepo.LeftLeaveByEmp(typeid , empid);

            if(result < 0)
            {
                return BadRequest(new
                {
                    success = false ,
                    message = "Error while getting result ..."

                
                });
            }
            else
            {
                return Ok(result);
            }



        }


        public async Task<IActionResult> Application([FromForm]Application app)
        {

            int? empid = HttpContext.Session.GetInt32("UserId");
            if(empid == null)
            {
                return BadRequest(1);
            }
                    app.EmpId = empid;
             int Leftleaves = await _empRepo.LeftLeaveByEmp(app.LeaveId , empid);

            if(Leftleaves < 0)
            {
                return BadRequest(new
                {
                    success = false ,
                    message = "Error while getting result ..."

                
                });
            }

            System.Console.WriteLine(app.EmpId);
            System.Console.WriteLine(app.LeaveId);
            var result = await _empRepo.Application(app);

            if(result == 0)
                {
                    return BadRequest(new{
                        success = false ,
                        message = "Erro while Adding Application leave"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = true ,
                        message = "Added sccess fully"
                    });
                }

             
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromForm]User user)
        {
            int? empid = HttpContext.Session.GetInt32("UserId");
            if (empid == null)
            {
                return BadRequest(1);
            }

            User? oldUser = await _empRepo.GetEmpById(empid.Value);
            if (oldUser == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            user.UserId = empid.Value;
            user.Role = oldUser.Role;
            user.Image = oldUser.Image;

            if (user.ProfilePic != null && user.ProfilePic.Length > 0)
            {
                string imageurl = Guid.NewGuid() + Path.GetExtension(user.ProfilePic.FileName);
                string folderpath = Path.Combine(_env.WebRootPath, "ProfileImages");

                if (!Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
                }

                string filepath = Path.Combine(folderpath, imageurl);
                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    user.ProfilePic.CopyTo(stream);
                }

                user.Image = imageurl;
            }

            int result = await _empRepo.UpdateEmp(user);
            if (result == 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error while updating profile"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Profile updated successfully"
            });
        }

    }
}
