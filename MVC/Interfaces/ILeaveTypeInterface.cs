namespace MVC;

public interface ILeaveTypeInterface
{
    public Task<List<LeaveType>> GetAllLeave();

    public Task<int> CreateLeave(LeaveType type);

    public Task<int> UpdateLeave(LeaveType type);

    public Task<int> DeleteLeave(int id);
}
