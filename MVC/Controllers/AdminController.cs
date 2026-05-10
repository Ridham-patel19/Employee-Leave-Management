using Microsoft.AspNetCore.Mvc;

namespace MVC.Namespace
{
    public class AdminController : Controller
    {
        // GET: AdminController
        
private readonly ILeaveTypeInterface _leave;

public AdminController(ILeaveTypeInterface leave)
{
    _leave = leave;
}
        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> GetAllLeave()
        {
            List<LeaveType> types = await _leave.GetAllLeave();

            if(types.Count > 0 && types != null)
            {
                return Ok(types);
            }
            else
            {
                return BadRequest(new
                {
                    success = false ,
                    message = "Error while getting all"
                });
            }


        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LeaveType type)
        {

            
            if(type == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Please eneter proper details"
                });

            }


            int result = await _leave.CreateLeave(type);

            if(result == 1)
            {
                return Ok(new
                {
                    success = true,
                    message = "inserted successfully"
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error in insertion"
                });
            }
        }


[HttpPost]
        public async Task<IActionResult> Update([FromBody]LeaveType type)
        {
             if(type == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Please eneter proper details"
                });

            }


            int result = await _leave.UpdateLeave(type);

            if(result == 1)
            {
                return Ok(new
                {
                    success = true,
                    message = "Updated successfully"
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error in Updation"
                });
            }
        }


[HttpPost]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            int result = await _leave.DeleteLeave(id);

            if(result > 0)
            {
                return Ok(new
                {
                    success = true,
                    messagw = "Deleted successfully"
                });


            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    messgae = "Error whole Deletion"
                });
            }


        }

    }
}
