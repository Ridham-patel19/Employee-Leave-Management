
using Npgsql;

namespace MVC;

public class LeaveTypeRepository : ILeaveTypeInterface
{

     private readonly NpgsqlConnection _conn;

    public LeaveTypeRepository(NpgsqlConnection conn)
    {
        _conn=conn;
        
    }
    public async Task<int> CreateLeave(LeaveType type)
    {
        string qry = "INSERT INTO t_leavetype (c_type , c_max) VALUES (@type , @max)";

        try
        {
            using var cmd = new NpgsqlCommand(qry , _conn);

            cmd.Parameters.AddWithValue("@type" , type.Type);
            cmd.Parameters.AddWithValue("@max" , type.MaxLeave);

            await _conn.OpenAsync();

            int result = await cmd.ExecuteNonQueryAsync();

            return result > 0 ? 1 : 0;


        }catch(Exception e)
        {
            System.Console.WriteLine("Error in type creation " + e.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> DeleteLeave(int id)
    {
        string qry = "DELETE FROM t_leavetype WHERE c_leaveid = @id";


        try
        {
            using var cmd = new NpgsqlCommand(qry , _conn);

            cmd.Parameters.AddWithValue("@id" , id);

            await _conn.OpenAsync();

            int result = await cmd.ExecuteNonQueryAsync();

            return result > 0 ? 1 : 0;

        }catch(Exception e)
        {
            System.Console.WriteLine("Error in deleting type " + e.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<List<LeaveType>> GetAllLeave()
    {

        string qry = "SELECT c_leaveid , c_type , c_max FROM t_leavetype";
         List<LeaveType> types = new List<LeaveType>();
        try
        {
            using var cmd = new NpgsqlCommand(qry , _conn);

            await _conn.OpenAsync();

            var reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                types.Add(new LeaveType
                {
                    LeaveId = reader.GetInt32(0),
                    Type = reader.GetString(1),
                    MaxLeave = reader.GetInt32(2)
                });
            }

            return types;
        }catch(Exception e)
        {
            System.Console.WriteLine("error while getting all leave " + e.Message);
            return null;
        }
        finally
        {
            await _conn.CloseAsync();
        }
        
    }

    public async Task<int> UpdateLeave(LeaveType type)
    {
        string qry = "UPDATE t_leavetype SET c_type = @type , c_max = @max WHERE c_leaveid = @id";

        try
        {
            using var cmd = new NpgsqlCommand(qry , _conn);
            cmd.Parameters.AddWithValue("@type" , type.Type);
            cmd.Parameters.AddWithValue("@max" , type.MaxLeave);
            cmd.Parameters.AddWithValue("@id" , type.LeaveId);

            await _conn.OpenAsync();

            int result = await cmd.ExecuteNonQueryAsync();

            return result > 0 ? 1 : 0;
        }catch(Exception e)
        {
            System.Console.WriteLine("Error in  update type  " + e.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }
}
