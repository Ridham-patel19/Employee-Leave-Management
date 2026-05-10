using System.ComponentModel.DataAnnotations;

namespace MVC;

public class User
{
  [Key]
  public int UserId { get; set; }


[StringLength(100)]
[Required(ErrorMessage = "Name is Required")]
  public string Name { get; set; }


[StringLength(100)]
[Required(ErrorMessage = "Email is Required")]
[EmailAddress(ErrorMessage = "Please enter valid email")]
  public string Email { get; set; }


[StringLength(100)]
[Required(ErrorMessage = "Passowrd is Required")]
[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$")]
  public string Password { get; set; }

  public string Role { get; set; } = "User";



[StringLength(100)]
[Required(ErrorMessage = "Gender is Required")]
[RegularExpression("Male|Female")]
  public string Gender { get; set; }

  public string? Image { get; set; }

  public IFormFile? ProfilePic { get; set; }
}
