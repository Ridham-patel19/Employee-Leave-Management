using Npgsql;
using MVC.Models;

namespace MVC;

public class EmpRepository : IEmpInterface
{
    private readonly NpgsqlConnection _conn;

    public EmpRepository(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    // ✅ CREATE
    public async Task<int> Create(Application app)
    {
        string qry = @"INSERT INTO t_leaveapplication
                       (c_empid, c_leaveid, c_startdate, c_enddate, c_reason)
                       VALUES (@empid, @leaveid, @start, @end, @reason)";

        try
        {
            using var cmd = new NpgsqlCommand(qry, _conn);

            cmd.Parameters.AddWithValue("@empid", app.EmpId);
            cmd.Parameters.AddWithValue("@leaveid", app.LeaveId);

            // 🔥 DateOnly → DateTime
            cmd.Parameters.AddWithValue("@start", app.StartDate.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@end", app.EndDate.ToDateTime(TimeOnly.MinValue));

            cmd.Parameters.AddWithValue("@reason", app.Reason);

            await _conn.OpenAsync();
            int result = await cmd.ExecuteNonQueryAsync();

            return result > 0 ? 1 : 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in create: " + e.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    // ✅ READ
    public async Task<List<Application>> GetAllById(int? id)
    {
        string qry = @"SELECT c_applicationid, c_empid, c_leaveid,
                              c_startdate, c_enddate, c_reason
                        FROM t_leaveapplication WHERE c_empid = @id";

        List<Application> list = new();

        try
        {
            using var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@id", id);

            await _conn.OpenAsync();
            var reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                list.Add(new Application
                {
                    ApplicationId = reader.GetInt32(0),
                    EmpId = reader.GetInt32(1),
                    LeaveId = reader.GetInt32(2),

                    // 🔥 DateTime → DateOnly
                    StartDate = DateOnly.FromDateTime(reader.GetDateTime(3)),
                    EndDate = DateOnly.FromDateTime(reader.GetDateTime(4)),

                    Reason = reader.GetString(5)
                });
            }

            return list;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in get all: " + e.Message);
            return null;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<Application?> GetApplicationById(int id, int? empid)
    {
        string qry = @"SELECT c_appid, c_empid, c_leaveid,
                              c_startdate, c_enddate, c_reason
                       FROM t_leaveapplication
                       WHERE c_appid = @id AND c_empid = @empid";

        try
        {
            using var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@empid", empid);

            await _conn.OpenAsync();
            var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Application
                {
                    ApplicationId = reader.GetInt32(0),
                    EmpId = reader.GetInt32(1),
                    LeaveId = reader.GetInt32(2),
                    StartDate = DateOnly.FromDateTime(reader.GetDateTime(3)),
                    EndDate = DateOnly.FromDateTime(reader.GetDateTime(4)),
                    Reason = reader.GetString(5)
                };
            }

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in get by id: " + e.Message);
            return null;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    // ✅ UPDATE
    public async Task<int> Update(Application app)
    {
        string qry = @"UPDATE t_leaveapplication
                       SET c_empid = @empid,
                           c_leaveid = @leaveid,
                           c_startdate = @start,
                           c_enddate = @end,
                           c_reason = @reason
                       WHERE c_appid = @id";

        try
        {
            using var cmd = new NpgsqlCommand(qry, _conn);

            cmd.Parameters.AddWithValue("@empid", app.EmpId);
            cmd.Parameters.AddWithValue("@leaveid", app.LeaveId);

            // 🔥 DateOnly → DateTime
            cmd.Parameters.AddWithValue("@start", app.StartDate.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@end", app.EndDate.ToDateTime(TimeOnly.MinValue));

            cmd.Parameters.AddWithValue("@reason", app.Reason);
            cmd.Parameters.AddWithValue("@id", app.ApplicationId);

            await _conn.OpenAsync();
            int result = await cmd.ExecuteNonQueryAsync();

            return result > 0 ? 1 : 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in update: " + e.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    // ✅ DELETE (POST style)
    public async Task<int> Delete(int id)
    {
        string qry = "DELETE FROM t_leaveapplication WHERE c_appid = @id";

        try
        {
            using var cmd = new NpgsqlCommand(qry, _conn);

            cmd.Parameters.AddWithValue("@id", id);

            await _conn.OpenAsync();
            int result = await cmd.ExecuteNonQueryAsync();

            return result > 0 ? 1 : 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in delete: " + e.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> LeftLeaveByEmp(int typeid, int? empid)
    {
        string qry = @"
SELECT 
    t.c_max - COALESCE(SUM((l.c_enddate - l.c_startdate) + 1), 0) AS leftLeaves
FROM t_leavetype t
LEFT JOIN t_leaveapplication l 
    ON l.c_leaveid = t.c_leaveid 
    AND l.c_empid = @empId
WHERE t.c_leaveid = @leaveId
GROUP BY t.c_leaveid, t.c_max;
";

        try
        {
            using var cmd = new NpgsqlCommand(qry, _conn);

            cmd.Parameters.AddWithValue("empId", empid);
            cmd.Parameters.AddWithValue("leaveId", typeid);


            await _conn.OpenAsync();

            int result = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            return result;


        }
        catch (Exception e)
        {
            System.Console.WriteLine("Error in getting left leave " + e.Message);
            return -1;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> Application(Application app)
    {
        string qry = @"INSERT INTO t_leaveapplication
(c_empid, c_leaveid, c_startdate, c_enddate, c_reason)
VALUES
(@c_empid, @c_leaveid, @c_startdate, @c_enddate, @c_reason)";

        try
        {
            using var cmd = new NpgsqlCommand(qry, _conn);

            cmd.Parameters.AddWithValue("@c_empid", app.EmpId == null ? DBNull.Value : app.EmpId);

            cmd.Parameters.AddWithValue("@c_leaveid", app.LeaveId);

            cmd.Parameters.AddWithValue("@c_startdate", app.StartDate);

            cmd.Parameters.AddWithValue("@c_enddate", app.EndDate);

            cmd.Parameters.AddWithValue("@c_reason", app.Reason);

            await _conn.OpenAsync();

            var result = await cmd.ExecuteNonQueryAsync();

            return result > 0 ? 1 : 0;

            
        }
        catch (Exception ex)
        {
            Console.WriteLine("error in application repo " + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<User?> GetEmpById(int id)
    {
        string qry = @"SELECT c_userid, c_username, c_email, c_password, c_role, c_image, c_gender
                       FROM t_user
                       WHERE c_userid = @id";

        try
        {
            using var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@id", id);

            await _conn.OpenAsync();
            var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    Role = reader.GetString(4),
                    Image = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Gender = reader.IsDBNull(6) ? "" : reader.GetString(6)
                };
            }

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in get emp by id: " + e.Message);
            return null;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> UpdateEmp(User user)
    {
        string qry = @"UPDATE t_user
                       SET c_username = @name,
                           c_email = @email,
                           c_password = @password,
                           c_image = @image,
                           c_gender = @gender
                       WHERE c_userid = @id";

        try
        {
            using var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@password", user.Password);
            cmd.Parameters.AddWithValue("@image", string.IsNullOrWhiteSpace(user.Image) ? DBNull.Value : user.Image);
            cmd.Parameters.AddWithValue("@gender", user.Gender);
            cmd.Parameters.AddWithValue("@id", user.UserId);

            await _conn.OpenAsync();
            int result = await cmd.ExecuteNonQueryAsync();

            return result > 0 ? 1 : 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in update emp: " + e.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }
}
