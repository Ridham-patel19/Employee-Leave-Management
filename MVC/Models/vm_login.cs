using System.ComponentModel.DataAnnotations;

namespace MVC;

public class vm_login
{
[StringLength(100)]
[Required(ErrorMessage = "Email is Required")]
[EmailAddress(ErrorMessage = "Please enter valid email")]
  public string Email { get; set; }


[StringLength(100)]
[Required(ErrorMessage = "Passowrd is Required")]
  public string Password { get; set; }
}

