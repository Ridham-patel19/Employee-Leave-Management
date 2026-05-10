using MVC.Models;

namespace MVC;

public interface IEmpInterface
{
    // ✅ Create new application
    Task<int> Create(Application app);

    // ✅ Get all applications by employee id
    Task<List<Application>> GetAllById(int? id);

    Task<Application?> GetApplicationById(int id, int? empid);

    // ✅ Update application
    Task<int> Update(Application app);

    // ✅ Delete application (POST style)
    Task<int> Delete(int id);

    Task<int> LeftLeaveByEmp(int typeid , int? empid);

    Task<int> Application(Application app);

    Task<User?> GetEmpById(int id);

    Task<int> UpdateEmp(User user);
}
