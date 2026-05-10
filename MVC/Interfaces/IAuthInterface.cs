namespace MVC;

public interface IAuthInterface
{
 public Task<int> Register(User user);
 public Task<User> Login(vm_login login);
}


