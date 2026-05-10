
using Microsoft.AspNetCore.Identity;
using Npgsql;

namespace MVC;

public class AuthRepository : IAuthInterface
{

    private readonly NpgsqlConnection _conn;

    public AuthRepository(NpgsqlConnection conn)
    {
        _conn=conn;
        
    }
    public async Task<User> Login(vm_login login)
    {
        
        string qry = "SELECT c_userid , c_username , c_email , c_password , c_role , c_image , c_gender FROM t_user WHERE c_email = @email AND c_password = @pass";

        try
        {
            using var cmd = new NpgsqlCommand(qry , _conn);
            cmd.Parameters.AddWithValue("@email" , login.Email);
            cmd.Parameters.AddWithValue("@pass",login.Password);

            await _conn.OpenAsync();

            var reader = await cmd.ExecuteReaderAsync();


            if (reader.Read())
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
            else
            {
                return null;
            }
        }catch(Exception e)
        {
            System.Console.WriteLine("Error while login : " + e.Message);
            return null;
        }
    }

    public async Task<int> Register(User user)
    {
        string qry = "INSERT INTO t_user (c_username , c_email , c_password , c_role , c_image , c_gender) VALUES (@username , @email , @password , @role , @image , @gender)";
            
        try
        {
            using var cmd = new NpgsqlCommand(qry , _conn);
             cmd.Parameters.AddWithValue("@username",user.Name!);
                cmd.Parameters.AddWithValue("@email",user.Email!);
                cmd.Parameters.AddWithValue("@password",user.Password!);
                cmd.Parameters.AddWithValue("@gender",user.Gender!);
                cmd.Parameters.AddWithValue("@role",user.Role!);
                cmd.Parameters.AddWithValue("@image",user.Image == null? DBNull.Value : user.Image);
                    await _conn.OpenAsync();
                int result = await cmd.ExecuteNonQueryAsync();

                return result > 0 ? 1 : 0;



                // cmd.Parameters.AddWithValue("@mobile",user.Mobile!);
                // cmd.Parameters.AddWithValue("@address",user.Address!);
                // cmd.Parameters.AddWithValue("@dob",user.DOB!);
        }catch(Exception e)
        {
            System.Console.WriteLine("There is an error  in register " + e.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }
}


